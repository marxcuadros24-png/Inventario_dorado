using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.DAL.Context;
using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.BLL.Services.Implementations;

public class RequerimientoService : IRequerimientoService
{
    private readonly InventarioDbContext _db;

    public RequerimientoService(InventarioDbContext db) => _db = db;

    public async Task<IEnumerable<Requerimiento>> GetTodosAsync()
        => await _db.Requerimientos
            .Include(r => r.Usuario).ThenInclude(u => u.Empleado)
            .Include(r => r.Detalles).ThenInclude(d => d.Herramienta)
            .Include(r => r.Detalles).ThenInclude(d => d.EPP)
            .OrderByDescending(r => r.Fecha)
            .AsNoTracking().ToListAsync();

    public async Task<Requerimiento?> GetPorIdAsync(int id)
        => await _db.Requerimientos
            .Include(r => r.Usuario).ThenInclude(u => u.Empleado)
            .Include(r => r.Detalles).ThenInclude(d => d.Herramienta).ThenInclude(h => h!.Unidad)
            .Include(r => r.Detalles).ThenInclude(d => d.EPP).ThenInclude(e => e!.Unidad)
            .FirstOrDefaultAsync(r => r.RequerimientoId == id);

    public async Task<IEnumerable<Requerimiento>> GetPendientesAsync()
        => await _db.Requerimientos
            .Include(r => r.Detalles).ThenInclude(d => d.Herramienta)
            .Include(r => r.Detalles).ThenInclude(d => d.EPP)
            .Where(r => r.Estado == EstadoRequerimiento.Pendiente)
            .OrderByDescending(r => r.Fecha)
            .AsNoTracking().ToListAsync();

    /// <summary>
    /// Genera automáticamente un Requerimiento con todos los ítems
    /// cuyo stock sea igual o inferior al StockMínimo.
    /// </summary>
    public async Task<(bool Exito, string Mensaje, Requerimiento? Req)> GenerarPorStockBajoAsync(
        int usuarioId, string? area = null)
    {
        var detalles = new List<DetalleRequerimiento>();

        // ── Herramientas con stock bajo ──────────────────────
        var herramientas = await _db.Herramientas
            .Include(h => h.Stock)
            .Where(h => h.Activo && h.StockMinimo > 0)
            .AsNoTracking().ToListAsync();

        foreach (var h in herramientas)
        {
            var stockActual = h.Stock?.CantidadActual ?? 0;
            if (stockActual <= h.StockMinimo)
            {
                detalles.Add(new DetalleRequerimiento
                {
                    TipoItem       = TipoItem.Herramienta,
                    HerramientaId  = h.HerramientaId,
                    StockActual    = stockActual,
                    StockMinimo    = h.StockMinimo,
                    CantidadSugerida = (h.StockMaximo ?? h.StockMinimo * 2) - stockActual
                });
            }
        }

        // ── EPP con stock bajo ───────────────────────────────
        var epps = await _db.EPPs
            .Where(e => e.Activo && e.StockMinimo > 0)
            .AsNoTracking().ToListAsync();

        foreach (var e in epps)
        {
            if (e.StockActual <= e.StockMinimo)
            {
                detalles.Add(new DetalleRequerimiento
                {
                    TipoItem       = TipoItem.EPP,
                    EPPId          = e.EPPId,
                    StockActual    = e.StockActual,
                    StockMinimo    = e.StockMinimo,
                    CantidadSugerida = (e.StockMaximo ?? e.StockMinimo * 2) - e.StockActual
                });
            }
        }

        if (!detalles.Any())
            return (false, "No hay ítems con stock bajo o mínimo en este momento.", null);

        var numero = await GenerarNumeroAsync();
        var req = new Requerimiento
        {
            Numero    = numero,
            UsuarioId = usuarioId,
            Area      = area,
            Detalles  = detalles
        };

        await _db.Requerimientos.AddAsync(req);
        await _db.SaveChangesAsync();

        // Cargar navegaciones para retornar
        var resultado = await GetPorIdAsync(req.RequerimientoId);
        return (true, $"Requerimiento {numero} generado con {detalles.Count} ítem(s).", resultado);
    }

    public async Task<(bool Exito, string Mensaje)> MarcarImpresionAsync(int requerimientoId)
    {
        var req = await _db.Requerimientos.FindAsync(requerimientoId);
        if (req is null) return (false, "Requerimiento no encontrado.");
        req.FechaImpresion = DateTime.Now;
        await _db.SaveChangesAsync();
        return (true, "Fecha de impresión registrada.");
    }

    public async Task<(bool Exito, string Mensaje)> AnularAsync(int requerimientoId)
    {
        var req = await _db.Requerimientos.FindAsync(requerimientoId);
        if (req is null) return (false, "Requerimiento no encontrado.");
        if (req.Estado == EstadoRequerimiento.Atendido)
            return (false, "No se puede anular un requerimiento ya atendido.");

        req.Estado = EstadoRequerimiento.Anulado;
        await _db.SaveChangesAsync();
        return (true, "Requerimiento anulado.");
    }

    public async Task<bool> HayItemsConStockBajoAsync()
    {
        var hayHerramientas = await _db.Herramientas
            .Include(h => h.Stock)
            .AnyAsync(h => h.Activo && h.StockMinimo > 0 &&
                           (h.Stock == null || h.Stock.CantidadActual <= h.StockMinimo));

        if (hayHerramientas) return true;

        return await _db.EPPs
            .AnyAsync(e => e.Activo && e.StockMinimo > 0 && e.StockActual <= e.StockMinimo);
    }

    private async Task<string> GenerarNumeroAsync()
    {
        var anio = DateTime.Now.Year;
        var count = await _db.Requerimientos.CountAsync(r => r.Fecha.Year == anio) + 1;
        return $"REQ-{anio}-{count:D6}";
    }
}
