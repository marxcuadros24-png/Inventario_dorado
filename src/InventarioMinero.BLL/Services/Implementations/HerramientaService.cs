using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.DAL.Context;
using InventarioMinero.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.BLL.Services.Implementations;

public class HerramientaService : IHerramientaService
{
    private readonly InventarioDbContext _db;

    public HerramientaService(InventarioDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Herramienta>> GetTodasAsync(bool soloActivas = true)
    {
        var query = _db.Herramientas
            .Include(h => h.Categoria)
            .Include(h => h.Unidad)
            .Include(h => h.Stock)
            .AsNoTracking();

        if (soloActivas)
            query = query.Where(h => h.Activo);

        return await query.OrderBy(h => h.Nombre).ToListAsync();
    }

    public async Task<Herramienta?> GetPorIdAsync(int id)
    {
        return await _db.Herramientas
            .Include(h => h.Categoria)
            .Include(h => h.Unidad)
            .Include(h => h.Stock)
            .FirstOrDefaultAsync(h => h.HerramientaId == id);
    }

    public async Task<Herramienta?> GetPorCodigoAsync(string codigo)
    {
        return await _db.Herramientas
            .Include(h => h.Categoria)
            .Include(h => h.Unidad)
            .Include(h => h.Stock)
            .FirstOrDefaultAsync(h => h.Codigo == codigo);
    }

    public async Task<(bool Exito, string Mensaje, Herramienta? Herramienta)> CrearAsync(Herramienta herramienta)
    {
        return await CrearConStockInicialAsync(herramienta, 0);
    }

    public async Task<(bool Exito, string Mensaje, Herramienta? Herramienta)> CrearConStockInicialAsync(
        Herramienta herramienta, decimal stockInicial)
    {
        if (string.IsNullOrWhiteSpace(herramienta.Nombre))
            return (false, "El nombre de la herramienta es obligatorio.", null);

        if (string.IsNullOrWhiteSpace(herramienta.Codigo))
            herramienta.Codigo = await GenerarCodigoAsync();

        if (await _db.Herramientas.AnyAsync(h => h.Codigo == herramienta.Codigo))
            return (false, $"El código '{herramienta.Codigo}' ya existe en el catálogo.", null);

        herramienta.FechaCreacion = DateTime.Now;
        herramienta.Activo = true;

        await _db.Herramientas.AddAsync(herramienta);
        await _db.SaveChangesAsync();

        // Crear registro de stock inicial correspondiente
        var stock = new StockHerramienta
        {
            HerramientaId = herramienta.HerramientaId,
            CantidadActual = Math.Max(0, stockInicial),
            CantidadReservada = 0,
            UltimaActualizacion = DateTime.Now
        };
        await _db.StocksHerramienta.AddAsync(stock);
        await _db.SaveChangesAsync();

        herramienta.Stock = stock;
        return (true, "Herramienta registrada exitosamente.", herramienta);
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarAsync(Herramienta herramienta)
    {
        var existing = await _db.Herramientas
            .Include(h => h.Stock)
            .FirstOrDefaultAsync(h => h.HerramientaId == herramienta.HerramientaId);

        if (existing == null)
            return (false, "Herramienta no encontrada.");

        existing.Nombre = herramienta.Nombre;
        existing.Descripcion = herramienta.Descripcion;
        existing.CategoriaId = herramienta.CategoriaId;
        existing.UnidadId = herramienta.UnidadId;
        existing.Marca = herramienta.Marca;
        existing.Modelo = herramienta.Modelo;
        existing.StockMinimo = herramienta.StockMinimo;
        existing.StockMaximo = herramienta.StockMaximo;
        existing.PrecioUnitario = herramienta.PrecioUnitario;
        existing.Activo = herramienta.Activo;

        await _db.SaveChangesAsync();
        return (true, "Herramienta actualizada correctamente.");
    }

    public async Task<(bool Exito, string Mensaje)> ActivarDesactivarAsync(int herramientaId)
    {
        var existing = await _db.Herramientas.FindAsync(herramientaId);
        if (existing == null)
            return (false, "Herramienta no encontrada.");

        existing.Activo = !existing.Activo;
        await _db.SaveChangesAsync();

        return (true, existing.Activo ? "Herramienta activada." : "Herramienta desactivada.");
    }

    public async Task<IEnumerable<Herramienta>> BuscarAsync(string termino)
    {
        return await BuscarAvanzadoAsync(termino, null, false);
    }

    public async Task<IEnumerable<Herramienta>> BuscarAvanzadoAsync(
        string? termino, int? categoriaId = null, bool soloBajoStock = false)
    {
        var query = _db.Herramientas
            .Include(h => h.Categoria)
            .Include(h => h.Unidad)
            .Include(h => h.Stock)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(termino))
        {
            var t = termino.Trim().ToLower();
            query = query.Where(h =>
                h.Codigo.ToLower().Contains(t) ||
                h.Nombre.ToLower().Contains(t) ||
                (h.Marca != null && h.Marca.ToLower().Contains(t)) ||
                (h.Modelo != null && h.Modelo.ToLower().Contains(t)));
        }

        if (categoriaId.HasValue && categoriaId.Value > 0)
        {
            query = query.Where(h => h.CategoriaId == categoriaId.Value);
        }

        if (soloBajoStock)
        {
            query = query.Where(h => h.Stock != null &&
                (h.Stock.CantidadActual - h.Stock.CantidadReservada) <= h.StockMinimo);
        }

        return await query.OrderBy(h => h.Nombre).ToListAsync();
    }

    public async Task<string> GenerarCodigoAsync()
    {
        var total = await _db.Herramientas.CountAsync();
        int siguiente = total + 1;
        string codigo;
        do
        {
            codigo = $"HTM-{siguiente:D3}";
            siguiente++;
        }
        while (await _db.Herramientas.AnyAsync(h => h.Codigo == codigo));

        return codigo;
    }
}
