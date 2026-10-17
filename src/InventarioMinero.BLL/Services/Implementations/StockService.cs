using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.DAL.Context;
using InventarioMinero.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.BLL.Services.Implementations;

public class StockService : IStockService
{
    private readonly InventarioDbContext _db;

    public StockService(InventarioDbContext db)
    {
        _db = db;
    }

    public async Task<StockHerramienta?> GetStockAsync(int herramientaId)
    {
        return await _db.StocksHerramienta
            .Include(s => s.Herramienta)
            .FirstOrDefaultAsync(s => s.HerramientaId == herramientaId);
    }

    public async Task<IEnumerable<StockHerramienta>> GetStocksConAlertaAsync()
    {
        return await _db.StocksHerramienta
            .Include(s => s.Herramienta)
            .Where(s => s.Herramienta.Activo && 
                       (s.CantidadActual - s.CantidadReservada) <= s.Herramienta.StockMinimo)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> HaySuficienteStockAsync(int herramientaId, decimal cantidad)
    {
        var stock = await _db.StocksHerramienta
            .FirstOrDefaultAsync(s => s.HerramientaId == herramientaId);

        if (stock == null) return false;

        return stock.CantidadDisponible >= cantidad;
    }

    public async Task ActualizarStockAsync(int herramientaId, decimal cantidad, bool esEntrada)
    {
        var stock = await _db.StocksHerramienta
            .FirstOrDefaultAsync(s => s.HerramientaId == herramientaId);

        if (stock == null)
        {
            stock = new StockHerramienta
            {
                HerramientaId = herramientaId,
                CantidadActual = esEntrada ? cantidad : 0,
                CantidadReservada = 0,
                UltimaActualizacion = DateTime.Now
            };
            await _db.StocksHerramienta.AddAsync(stock);
        }
        else
        {
            if (esEntrada)
            {
                stock.CantidadActual += cantidad;
            }
            else
            {
                stock.CantidadActual = Math.Max(0, stock.CantidadActual - cantidad);
            }
            stock.UltimaActualizacion = DateTime.Now;
        }

        await _db.SaveChangesAsync();
    }

    public async Task ReservarStockAsync(int herramientaId, decimal cantidad)
    {
        var stock = await _db.StocksHerramienta
            .FirstOrDefaultAsync(s => s.HerramientaId == herramientaId);

        if (stock != null)
        {
            stock.CantidadReservada += cantidad;
            stock.UltimaActualizacion = DateTime.Now;
            await _db.SaveChangesAsync();
        }
    }

    public async Task LiberarReservaAsync(int herramientaId, decimal cantidad)
    {
        var stock = await _db.StocksHerramienta
            .FirstOrDefaultAsync(s => s.HerramientaId == herramientaId);

        if (stock != null)
        {
            stock.CantidadReservada = Math.Max(0, stock.CantidadReservada - cantidad);
            stock.UltimaActualizacion = DateTime.Now;
            await _db.SaveChangesAsync();
        }
    }
}
