using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.DAL.Context;
using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.BLL.Services.Implementations;

public class AsignacionService : IAsignacionService
{
    private readonly InventarioDbContext _db;
    private readonly IStockService _stockService;

    public AsignacionService(InventarioDbContext db, IStockService stockService)
    {
        _db = db;
        _stockService = stockService;
    }

    public async Task<IEnumerable<AsignacionHerramienta>> GetActivasAsync()
    {
        return await _db.AsignacionesHerramienta
            .Include(a => a.Herramienta)
                .ThenInclude(h => h.Unidad)
            .Include(a => a.Empleado)
                .ThenInclude(e => e.Cargo)
            .Include(a => a.Usuario)
            .Where(a => a.Estado == EstadoAsignacion.Activo)
            .OrderByDescending(a => a.FechaAsignacion)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<AsignacionHerramienta>> GetPorEmpleadoAsync(int empleadoId)
    {
        return await _db.AsignacionesHerramienta
            .Include(a => a.Herramienta)
                .ThenInclude(h => h.Unidad)
            .Include(a => a.Empleado)
            .Include(a => a.Usuario)
            .Where(a => a.EmpleadoId == empleadoId)
            .OrderByDescending(a => a.FechaAsignacion)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<(bool Exito, string Mensaje, string? Numero)> AsignarAsync(
        int herramientaId, int empleadoId, decimal cantidad,
        int usuarioId, DateTime? fechaDevolucion = null, string? observaciones = null)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            if (cantidad <= 0)
                return (false, "La cantidad a prestar debe ser mayor a cero.", null);

            var emp = await _db.Empleados.FindAsync(empleadoId);
            if (emp == null || !emp.Activo)
                return (false, "El trabajador seleccionado no existe o está inactivo.", null);

            var her = await _db.Herramientas.FindAsync(herramientaId);
            if (her == null || !her.Activo)
                return (false, "La herramienta seleccionada no existe o está inactiva.", null);

            var stock = await _db.StocksHerramienta.FirstOrDefaultAsync(s => s.HerramientaId == herramientaId);
            if (stock == null)
            {
                stock = new StockHerramienta
                {
                    HerramientaId = herramientaId,
                    CantidadActual = 0,
                    CantidadReservada = 0,
                    UltimaActualizacion = DateTime.Now
                };
                await _db.StocksHerramienta.AddAsync(stock);
                await _db.SaveChangesAsync();
            }

            if (stock.CantidadDisponible < cantidad)
            {
                return (false, $"Stock disponible insuficiente para '{her.Nombre}'. Disponible: {stock.CantidadDisponible:N2}, Solicitado: {cantidad:N2}.", null);
            }

            // Generar correlativo
            var anio = DateTime.Now.Year;
            var prefijo = $"ASG-{anio}-";
            var maxId = await _db.AsignacionesHerramienta
                .Where(a => a.Numero.StartsWith(prefijo))
                .CountAsync();
            var numeroAsignacion = $"{prefijo}{(maxId + 1):D6}";

            // Reservar stock
            stock.CantidadReservada += cantidad;
            stock.UltimaActualizacion = DateTime.Now;
            _db.StocksHerramienta.Update(stock);

            var asignacion = new AsignacionHerramienta
            {
                Numero = numeroAsignacion,
                HerramientaId = herramientaId,
                EmpleadoId = empleadoId,
                Cantidad = cantidad,
                FechaAsignacion = DateTime.Now,
                FechaDevProgramada = fechaDevolucion ?? DateTime.Now.AddDays(1),
                CondicionSalida = CondicionHerramienta.Bueno,
                UsuarioId = usuarioId,
                Observaciones = observaciones,
                Estado = EstadoAsignacion.Activo
            };

            await _db.AsignacionesHerramienta.AddAsync(asignacion);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, $"Préstamo registrado exitosamente bajo el código {numeroAsignacion}.", numeroAsignacion);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Error al registrar asignación: {ex.Message}", null);
        }
    }

    public async Task<(bool Exito, string Mensaje)> RegistrarDevolucionAsync(
        int asignacionId, string condicionRetorno, string? observaciones = null)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var asig = await _db.AsignacionesHerramienta
                .Include(a => a.Herramienta)
                .Include(a => a.Empleado)
                .FirstOrDefaultAsync(a => a.AsignacionId == asignacionId);

            if (asig == null)
                return (false, "El registro de préstamo no fue encontrado.");

            if (asig.Estado != EstadoAsignacion.Activo)
                return (false, $"Esta asignación ya fue procesada anteriormente con estado: {asig.Estado}.");

            var stock = await _db.StocksHerramienta.FirstOrDefaultAsync(s => s.HerramientaId == asig.HerramientaId);
            if (stock == null)
                return (false, "No se encontró el registro de stock de la herramienta.");

            CondicionHerramienta cond = CondicionHerramienta.Bueno;
            if (Enum.TryParse<CondicionHerramienta>(condicionRetorno, true, out var parsedCond))
            {
                cond = parsedCond;
            }

            asig.CondicionRetorno = cond;
            asig.FechaDevReal = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(observaciones))
            {
                asig.Observaciones = string.IsNullOrWhiteSpace(asig.Observaciones)
                    ? $"Retorno: {observaciones}"
                    : $"{asig.Observaciones} | Retorno: {observaciones}";
            }

            if (cond == CondicionHerramienta.Perdido)
            {
                asig.Estado = EstadoAsignacion.Perdido;
                // Si se perdió, sale definitivamente del stock físico y de la reserva
                stock.CantidadReservada = Math.Max(0, stock.CantidadReservada - asig.Cantidad);
                stock.CantidadActual = Math.Max(0, stock.CantidadActual - asig.Cantidad);
            }
            else
            {
                asig.Estado = EstadoAsignacion.Devuelto;
                // Si retorna, se libera de la reserva y vuelve al stock disponible
                stock.CantidadReservada = Math.Max(0, stock.CantidadReservada - asig.Cantidad);
            }

            stock.UltimaActualizacion = DateTime.Now;
            _db.StocksHerramienta.Update(stock);
            _db.AsignacionesHerramienta.Update(asig);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, $"Devolución registrada exitosamente. Herramienta reingresada en condición: {cond}.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Error al procesar devolución: {ex.Message}");
        }
    }
}
