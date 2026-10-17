using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.DAL.Context;
using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.BLL.Services.Implementations;

public class MovimientoService : IMovimientoService
{
    private readonly InventarioDbContext _db;
    private readonly IStockService _stockService;

    public MovimientoService(InventarioDbContext db, IStockService stockService)
    {
        _db = db;
        _stockService = stockService;
    }

    public async Task<IEnumerable<Movimiento>> GetTodosAsync(DateTime? desde = null, DateTime? hasta = null)
    {
        var query = _db.Movimientos
            .Include(m => m.TipoMovimiento)
            .Include(m => m.Usuario).ThenInclude(u => u.Empleado)
            .Include(m => m.Requerimiento)
            .Include(m => m.Detalles).ThenInclude(d => d.Herramienta)
            .AsNoTracking();

        if (desde.HasValue) query = query.Where(m => m.Fecha >= desde.Value);
        if (hasta.HasValue) query = query.Where(m => m.Fecha <= hasta.Value);

        return await query.OrderByDescending(m => m.Fecha).ToListAsync();
    }

    public async Task<Movimiento?> GetPorIdAsync(int id)
    {
        return await _db.Movimientos
            .Include(m => m.TipoMovimiento)
            .Include(m => m.Usuario).ThenInclude(u => u.Empleado)
            .Include(m => m.Requerimiento)
            .Include(m => m.Detalles).ThenInclude(d => d.Herramienta).ThenInclude(h => h.Unidad)
            .FirstOrDefaultAsync(m => m.MovimientoId == id);
    }

    public async Task<IEnumerable<TipoMovimiento>> GetTiposMovimientoAsync()
    {
        return await _db.TiposMovimiento
            .Where(t => t.Activo)
            .OrderBy(t => t.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Movimiento>> BuscarAvanzadoAsync(
        string? busqueda, int? tipoMovId, DateTime? desde, DateTime? hasta)
    {
        var query = _db.Movimientos
            .Include(m => m.TipoMovimiento)
            .Include(m => m.Usuario).ThenInclude(u => u.Empleado)
            .Include(m => m.Requerimiento)
            .Include(m => m.Detalles).ThenInclude(d => d.Herramienta)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var b = busqueda.Trim().ToLower();
            query = query.Where(m =>
                m.Numero.ToLower().Contains(b) ||
                (m.DocumentoRef != null && m.DocumentoRef.ToLower().Contains(b)) ||
                (m.Observaciones != null && m.Observaciones.ToLower().Contains(b)) ||
                m.Detalles.Any(d => d.Herramienta.Nombre.ToLower().Contains(b) || d.Herramienta.Codigo.ToLower().Contains(b)));
        }

        if (tipoMovId.HasValue && tipoMovId.Value > 0)
        {
            query = query.Where(m => m.TipoMovId == tipoMovId.Value);
        }

        if (desde.HasValue)
        {
            var fDesde = desde.Value.Date;
            query = query.Where(m => m.Fecha >= fDesde);
        }

        if (hasta.HasValue)
        {
            var fHasta = hasta.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(m => m.Fecha <= fHasta);
        }

        return await query.OrderByDescending(m => m.Fecha).ToListAsync();
    }

    public async Task<IEnumerable<DetalleMovimiento>> GetKardexPorHerramientaAsync(int herramientaId)
    {
        return await _db.DetallesMovimiento
            .Include(d => d.Movimiento).ThenInclude(m => m.TipoMovimiento)
            .Include(d => d.Movimiento).ThenInclude(m => m.Usuario).ThenInclude(u => u.Empleado)
            .Include(d => d.Herramienta).ThenInclude(h => h.Unidad)
            .Where(d => d.HerramientaId == herramientaId && d.Movimiento.Estado == EstadoMovimiento.Confirmado)
            .OrderByDescending(d => d.Movimiento.Fecha)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<(bool Exito, string Mensaje, string? Numero)> RegistrarAsync(
        int tipoMovId, int usuarioId, List<(int HerramientaId, decimal Cantidad, decimal Precio)> items,
        string? documentoRef = null, string? observaciones = null, int? requerimientoId = null)
    {
        if (items == null || !items.Any())
            return (false, "Debe agregar al menos una herramienta al movimiento.", null);

        var tipoMov = await _db.TiposMovimiento.FindAsync(tipoMovId);
        if (tipoMov == null)
            return (false, "Tipo de movimiento no válido.", null);

        // Validación de stock en caso de salida
        if (!tipoMov.EsEntrada)
        {
            foreach (var item in items)
            {
                if (item.Cantidad <= 0)
                    return (false, "La cantidad de salida debe ser mayor a cero.", null);

                var disponible = await _stockService.HaySuficienteStockAsync(item.HerramientaId, item.Cantidad);
                if (!disponible)
                {
                    var htm = await _db.Herramientas.FindAsync(item.HerramientaId);
                    var stock = await _stockService.GetStockAsync(item.HerramientaId);
                    return (false, 
                        $"Stock insuficiente para '{htm?.Nombre ?? "Herramienta"}'. Disponible: {stock?.CantidadDisponible ?? 0}, Solicitado: {item.Cantidad}.", 
                        null);
                }
            }
        }

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var numero = await GenerarNumeroAsync();

            var mov = new Movimiento
            {
                Numero = numero,
                TipoMovId = tipoMovId,
                UsuarioId = usuarioId,
                DocumentoRef = documentoRef?.Trim(),
                Observaciones = observaciones?.Trim(),
                RequerimientoId = requerimientoId,
                Fecha = DateTime.Now,
                Estado = EstadoMovimiento.Confirmado
            };

            await _db.Movimientos.AddAsync(mov);
            await _db.SaveChangesAsync();

            // Agregar detalles y actualizar stock
            foreach (var item in items)
            {
                var detalle = new DetalleMovimiento
                {
                    MovimientoId = mov.MovimientoId,
                    HerramientaId = item.HerramientaId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio
                };
                await _db.DetallesMovimiento.AddAsync(detalle);

                // Afectar el stock real
                await _stockService.ActualizarStockAsync(item.HerramientaId, item.Cantidad, tipoMov.EsEntrada);
            }

            await _db.SaveChangesAsync();

            // Si vino asociado a un requerimiento de compra, marcarlo como atendido
            if (requerimientoId.HasValue)
            {
                var req = await _db.Requerimientos.FindAsync(requerimientoId.Value);
                if (req != null)
                {
                    req.Estado = EstadoRequerimiento.Atendido;
                    req.FechaAtencion = DateTime.Now;
                    req.MovimientoId = mov.MovimientoId;
                    await _db.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();

            var accion = tipoMov.EsEntrada ? "Ingreso" : "Salida";
            return (true, $"{accion} registrada exitosamente bajo el código {numero}.", numero);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Error al registrar movimiento: {ex.Message}", null);
        }
    }

    public async Task<(bool Exito, string Mensaje)> AnularAsync(int movimientoId, int usuarioId)
    {
        var mov = await _db.Movimientos
            .Include(m => m.TipoMovimiento)
            .Include(m => m.Detalles)
            .FirstOrDefaultAsync(m => m.MovimientoId == movimientoId);

        if (mov == null)
            return (false, "Movimiento no encontrado.");

        if (mov.Estado == EstadoMovimiento.Anulado)
            return (false, "El movimiento ya se encuentra anulado.");

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // Revertir el efecto en stock:
            // Si era Entrada -> ahora restamos.
            // Si era Salida -> ahora sumamos.
            bool revertirComoEntrada = !mov.TipoMovimiento.EsEntrada;

            foreach (var det in mov.Detalles)
            {
                await _stockService.ActualizarStockAsync(det.HerramientaId, det.Cantidad, revertirComoEntrada);
            }

            mov.Estado = EstadoMovimiento.Anulado;
            mov.FechaAnulacion = DateTime.Now;
            mov.AnuladoPor = usuarioId;

            // Si atendió un requerimiento, volverlo a dejar en Pendiente
            if (mov.RequerimientoId.HasValue)
            {
                var req = await _db.Requerimientos.FindAsync(mov.RequerimientoId.Value);
                if (req != null)
                {
                    req.Estado = EstadoRequerimiento.Pendiente;
                    req.FechaAtencion = null;
                    req.MovimientoId = null;
                }
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, $"Movimiento {mov.Numero} anulado y stock revertido correctamente.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Error al anular movimiento: {ex.Message}");
        }
    }

    private async Task<string> GenerarNumeroAsync()
    {
        var anio = DateTime.Now.Year;
        var count = await _db.Movimientos.CountAsync(m => m.Fecha.Year == anio) + 1;
        return $"MOV-{anio}-{count:D6}";
    }
}
