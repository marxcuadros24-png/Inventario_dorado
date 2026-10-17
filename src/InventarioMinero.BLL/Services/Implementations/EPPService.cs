using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.DAL.Context;
using InventarioMinero.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.BLL.Services.Implementations;

public class EPPService : IEPPService
{
    private readonly InventarioDbContext _db;

    public EPPService(InventarioDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<EPP>> GetTodosAsync(bool soloActivos = true)
    {
        var query = _db.EPPs
            .Include(e => e.Categoria)
            .Include(e => e.Unidad)
            .AsNoTracking();

        if (soloActivos)
            query = query.Where(e => e.Activo);

        return await query.OrderBy(e => e.Nombre).ToListAsync();
    }

    public async Task<EPP?> GetPorIdAsync(int id)
    {
        return await _db.EPPs
            .Include(e => e.Categoria)
            .Include(e => e.Unidad)
            .FirstOrDefaultAsync(e => e.EPPId == id);
    }

    public async Task<(bool Exito, string Mensaje, EPP? EPP)> CrearAsync(EPP epp)
    {
        if (string.IsNullOrWhiteSpace(epp.Nombre))
            return (false, "El nombre del EPP es obligatorio.", null);

        if (string.IsNullOrWhiteSpace(epp.Codigo))
            epp.Codigo = await GenerarCodigoAsync();

        if (await _db.EPPs.AnyAsync(e => e.Codigo == epp.Codigo))
            return (false, $"El código '{epp.Codigo}' ya existe en el catálogo de EPP.", null);

        epp.FechaCreacion = DateTime.Now;
        epp.Activo = true;

        await _db.EPPs.AddAsync(epp);
        await _db.SaveChangesAsync();

        return (true, "Equipo EPP registrado exitosamente.", epp);
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarAsync(EPP epp)
    {
        var existing = await _db.EPPs.FindAsync(epp.EPPId);
        if (existing == null)
            return (false, "Equipo EPP no encontrado.");

        existing.Nombre = epp.Nombre;
        existing.Descripcion = epp.Descripcion;
        existing.CategoriaId = epp.CategoriaId;
        existing.UnidadId = epp.UnidadId;
        existing.Marca = epp.Marca;
        existing.Talla = epp.Talla;
        existing.VidaUtilMeses = epp.VidaUtilMeses;
        existing.StockMinimo = epp.StockMinimo;
        existing.StockMaximo = epp.StockMaximo;
        existing.StockActual = epp.StockActual;
        existing.PrecioUnitario = epp.PrecioUnitario;
        existing.Activo = epp.Activo;

        await _db.SaveChangesAsync();
        return (true, "Equipo EPP actualizado correctamente.");
    }

    public async Task<(bool Exito, string Mensaje)> ActivarDesactivarAsync(int eppId)
    {
        var existing = await _db.EPPs.FindAsync(eppId);
        if (existing == null)
            return (false, "Equipo EPP no encontrado.");

        existing.Activo = !existing.Activo;
        await _db.SaveChangesAsync();

        return (true, existing.Activo ? "Equipo EPP activado." : "Equipo EPP desactivado.");
    }

    public async Task<IEnumerable<EPP>> BuscarAvanzadoAsync(
        string? termino, int? categoriaId = null, bool soloBajoStock = false)
    {
        var query = _db.EPPs
            .Include(e => e.Categoria)
            .Include(e => e.Unidad)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(termino))
        {
            var t = termino.Trim().ToLower();
            query = query.Where(e =>
                e.Codigo.ToLower().Contains(t) ||
                e.Nombre.ToLower().Contains(t) ||
                (e.Marca != null && e.Marca.ToLower().Contains(t)) ||
                (e.Talla != null && e.Talla.ToLower().Contains(t)));
        }

        if (categoriaId.HasValue && categoriaId.Value > 0)
        {
            query = query.Where(e => e.CategoriaId == categoriaId.Value);
        }

        if (soloBajoStock)
        {
            query = query.Where(e => e.StockActual <= e.StockMinimo);
        }

        return await query.OrderBy(e => e.Nombre).ToListAsync();
    }

    public async Task<string> GenerarCodigoAsync()
    {
        var total = await _db.EPPs.CountAsync();
        int siguiente = total + 1;
        string codigo;
        do
        {
            codigo = $"EPP-{siguiente:D3}";
            siguiente++;
        }
        while (await _db.EPPs.AnyAsync(e => e.Codigo == codigo));

        return codigo;
    }
}
