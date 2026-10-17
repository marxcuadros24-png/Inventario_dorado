using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.DAL.Context;
using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.BLL.Services.Implementations;

public class CatalogoService : ICatalogoService
{
    private readonly InventarioDbContext _db;

    public CatalogoService(InventarioDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Categoria>> GetCategoriasAsync(TipoCategoria? tipo = null)
    {
        var query = _db.Categorias.AsNoTracking();
        if (tipo.HasValue)
        {
            query = query.Where(c => c.Tipo == tipo.Value || c.Tipo == TipoCategoria.Ambos);
        }
        return await query.OrderBy(c => c.Nombre).ToListAsync();
    }

    public async Task<IEnumerable<UnidadMedida>> GetUnidadesMedidaAsync()
    {
        return await _db.UnidadesMedida
            .AsNoTracking()
            .OrderBy(u => u.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Cargo>> GetCargosAsync()
    {
        return await _db.Cargos
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rol>> GetRolesAsync()
    {
        return await _db.Roles
            .AsNoTracking()
            .OrderBy(r => r.RolId)
            .ToListAsync();
    }
}
