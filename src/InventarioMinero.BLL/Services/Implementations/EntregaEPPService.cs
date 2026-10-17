using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.DAL.Context;
using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.BLL.Services.Implementations;

public class EntregaEPPService : IEntregaEPPService
{
    private readonly InventarioDbContext _db;

    public EntregaEPPService(InventarioDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<EntregaEPP>> GetTodasAsync(DateTime? desde = null, DateTime? hasta = null)
    {
        var query = _db.EntregasEPP
            .Include(e => e.Empleado)
                .ThenInclude(emp => emp.Cargo)
            .Include(e => e.Usuario)
            .Include(e => e.Detalles)
                .ThenInclude(d => d.EPP)
                    .ThenInclude(ep => ep.Unidad)
            .AsNoTracking();

        if (desde.HasValue)
            query = query.Where(e => e.Fecha >= desde.Value.Date);

        if (hasta.HasValue)
            query = query.Where(e => e.Fecha <= hasta.Value.Date.AddDays(1).AddTicks(-1));

        return await query.OrderByDescending(e => e.Fecha).ToListAsync();
    }

    public async Task<IEnumerable<EntregaEPP>> GetPorEmpleadoAsync(int empleadoId)
    {
        return await _db.EntregasEPP
            .Include(e => e.Empleado)
                .ThenInclude(emp => emp.Cargo)
            .Include(e => e.Usuario)
            .Include(e => e.Detalles)
                .ThenInclude(d => d.EPP)
                    .ThenInclude(ep => ep.Unidad)
            .Where(e => e.EmpleadoId == empleadoId)
            .OrderByDescending(e => e.Fecha)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<(bool Exito, string Mensaje, string? Numero)> RegistrarAsync(
        int empleadoId, int usuarioId, string motivo,
        List<(int EPPId, decimal Cantidad, decimal Precio, DateTime? FechaVencimiento)> items,
        string? observaciones = null)
    {
        if (items == null || items.Count == 0)
            return (false, "Debe agregar al menos un EPP a la entrega.", null);

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var emp = await _db.Empleados.FindAsync(empleadoId);
            if (emp == null || !emp.Activo)
                return (false, "El trabajador seleccionado no existe o está inactivo.", null);

            MotivoEntregaEPP motivoParsed = MotivoEntregaEPP.Dotacion;
            if (Enum.TryParse<MotivoEntregaEPP>(motivo, true, out var pMotivo))
                motivoParsed = pMotivo;

            // Generar correlativo
            var anio = DateTime.Now.Year;
            var prefijo = $"ENT-EPP-{anio}-";
            var maxId = await _db.EntregasEPP
                .Where(e => e.Numero.StartsWith(prefijo))
                .CountAsync();
            var numeroEntrega = $"{prefijo}{(maxId + 1):D6}";

            var entrega = new EntregaEPP
            {
                Numero = numeroEntrega,
                EmpleadoId = empleadoId,
                UsuarioId = usuarioId,
                Fecha = DateTime.Now,
                Motivo = motivoParsed,
                FirmaEmpleado = true,
                Observaciones = observaciones,
                Anulado = false
            };

            await _db.EntregasEPP.AddAsync(entrega);
            await _db.SaveChangesAsync();

            // Validar stock de cada EPP y descontar
            foreach (var item in items)
            {
                if (item.Cantidad <= 0)
                    throw new InvalidOperationException("La cantidad de entrega debe ser mayor a 0.");

                var epp = await _db.EPPs.FindAsync(item.EPPId);
                if (epp == null || !epp.Activo)
                    throw new InvalidOperationException($"El EPP con ID {item.EPPId} no existe o no está activo.");

                if (epp.StockActual < item.Cantidad)
                {
                    throw new InvalidOperationException(
                        $"Stock insuficiente para el EPP '{epp.Nombre}'. Disponible: {epp.StockActual:N0}, Solicitado: {item.Cantidad:N0}.");
                }

                // Descontar stock de EPP
                epp.StockActual -= item.Cantidad;
                _db.EPPs.Update(epp);

                // Calcular vencimiento estimado según vida útil del EPP
                var mesesVida = epp.VidaUtilMeses.GetValueOrDefault(6);
                var vencimiento = item.FechaVencimiento ?? DateTime.Now.AddMonths(mesesVida);

                var detalle = new DetalleEntregaEPP
                {
                    EntregaId = entrega.EntregaId,
                    EPPId = item.EPPId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio > 0 ? item.Precio : epp.PrecioUnitario,
                    FechaVencimiento = vencimiento,
                    Observaciones = $"Dotación válida por {mesesVida} meses"
                };

                await _db.DetallesEntregaEPP.AddAsync(detalle);
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, $"Dotación de EPP registrada con éxito bajo el código {numeroEntrega}.", numeroEntrega);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Error al registrar entrega de EPP: {ex.Message}", null);
        }
    }
}
