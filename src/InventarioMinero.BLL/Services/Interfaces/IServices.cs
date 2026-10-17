using InventarioMinero.Models.Entities;

namespace InventarioMinero.BLL.Services.Interfaces;

public interface IUsuarioService
{
    Task<(bool Exito, Usuario? Usuario, string Mensaje)> AutenticarAsync(string nombreUsuario, string password);
    Task<IEnumerable<Usuario>> GetTodosAsync();
    Task<Usuario?> GetPorIdAsync(int id);
    Task<(bool Exito, string Mensaje)> CrearAsync(Usuario usuario, string passwordPlano);
    Task<(bool Exito, string Mensaje)> ActualizarAsync(Usuario usuario);
    Task<(bool Exito, string Mensaje)> CambiarPasswordAsync(int usuarioId, string passwordActual, string passwordNuevo);
    Task<(bool Exito, string Mensaje)> ActivarDesactivarAsync(int usuarioId);
    Task RegistrarAccesoAsync(int usuarioId);
}

public interface IHerramientaService
{
    Task<IEnumerable<Herramienta>> GetTodasAsync(bool soloActivas = true);
    Task<Herramienta?> GetPorIdAsync(int id);
    Task<Herramienta?> GetPorCodigoAsync(string codigo);
    Task<(bool Exito, string Mensaje, Herramienta? Herramienta)> CrearAsync(Herramienta herramienta);
    Task<(bool Exito, string Mensaje)> ActualizarAsync(Herramienta herramienta);
    Task<(bool Exito, string Mensaje)> ActivarDesactivarAsync(int herramientaId);
    Task<IEnumerable<Herramienta>> BuscarAsync(string termino);
    Task<IEnumerable<Herramienta>> BuscarAvanzadoAsync(string? termino, int? categoriaId = null, bool soloBajoStock = false);
    Task<(bool Exito, string Mensaje, Herramienta? Herramienta)> CrearConStockInicialAsync(Herramienta herramienta, decimal stockInicial);
    Task<string> GenerarCodigoAsync();
}

public interface IStockService
{
    Task<StockHerramienta?> GetStockAsync(int herramientaId);
    Task<IEnumerable<StockHerramienta>> GetStocksConAlertaAsync();
    Task<bool> HaySuficienteStockAsync(int herramientaId, decimal cantidad);
    Task ActualizarStockAsync(int herramientaId, decimal cantidad, bool esEntrada);
    Task ReservarStockAsync(int herramientaId, decimal cantidad);
    Task LiberarReservaAsync(int herramientaId, decimal cantidad);
}

public interface IMovimientoService
{
    Task<IEnumerable<Movimiento>> GetTodosAsync(DateTime? desde = null, DateTime? hasta = null);
    Task<Movimiento?> GetPorIdAsync(int id);
    Task<(bool Exito, string Mensaje, string? Numero)> RegistrarAsync(
        int tipoMovId, int usuarioId, List<(int HerramientaId, decimal Cantidad, decimal Precio)> items,
        string? documentoRef = null, string? observaciones = null, int? requerimientoId = null);
    Task<(bool Exito, string Mensaje)> AnularAsync(int movimientoId, int usuarioId);
    Task<IEnumerable<TipoMovimiento>> GetTiposMovimientoAsync();
    Task<IEnumerable<Movimiento>> BuscarAvanzadoAsync(string? busqueda, int? tipoMovId, DateTime? desde, DateTime? hasta);
    Task<IEnumerable<DetalleMovimiento>> GetKardexPorHerramientaAsync(int herramientaId);
}

public interface IAsignacionService
{
    Task<IEnumerable<AsignacionHerramienta>> GetActivasAsync();
    Task<IEnumerable<AsignacionHerramienta>> GetPorEmpleadoAsync(int empleadoId);
    Task<(bool Exito, string Mensaje, string? Numero)> AsignarAsync(
        int herramientaId, int empleadoId, decimal cantidad,
        int usuarioId, DateTime? fechaDevolucion = null, string? observaciones = null);
    Task<(bool Exito, string Mensaje)> RegistrarDevolucionAsync(
        int asignacionId, string condicionRetorno, string? observaciones = null);
}

public interface IEPPService
{
    Task<IEnumerable<EPP>> GetTodosAsync(bool soloActivos = true);
    Task<EPP?> GetPorIdAsync(int id);
    Task<(bool Exito, string Mensaje, EPP? EPP)> CrearAsync(EPP epp);
    Task<(bool Exito, string Mensaje)> ActualizarAsync(EPP epp);
    Task<(bool Exito, string Mensaje)> ActivarDesactivarAsync(int eppId);
    Task<IEnumerable<EPP>> BuscarAvanzadoAsync(string? termino, int? categoriaId = null, bool soloBajoStock = false);
    Task<string> GenerarCodigoAsync();
}

public interface ICatalogoService
{
    Task<IEnumerable<Categoria>> GetCategoriasAsync(InventarioMinero.Models.Enums.TipoCategoria? tipo = null);
    Task<IEnumerable<UnidadMedida>> GetUnidadesMedidaAsync();
    Task<IEnumerable<Cargo>> GetCargosAsync();
    Task<IEnumerable<Rol>> GetRolesAsync();
}

public interface IEntregaEPPService
{
    Task<IEnumerable<EntregaEPP>> GetTodasAsync(DateTime? desde = null, DateTime? hasta = null);
    Task<IEnumerable<EntregaEPP>> GetPorEmpleadoAsync(int empleadoId);
    Task<(bool Exito, string Mensaje, string? Numero)> RegistrarAsync(
        int empleadoId, int usuarioId, string motivo,
        List<(int EPPId, decimal Cantidad, decimal Precio, DateTime? FechaVencimiento)> items,
        string? observaciones = null);
}

public interface IRequerimientoService
{
    Task<IEnumerable<Requerimiento>> GetTodosAsync();
    Task<Requerimiento?> GetPorIdAsync(int id);
    Task<IEnumerable<Requerimiento>> GetPendientesAsync();
    Task<(bool Exito, string Mensaje, Requerimiento? Req)> GenerarPorStockBajoAsync(int usuarioId, string? area = null);
    Task<(bool Exito, string Mensaje)> MarcarImpresionAsync(int requerimientoId);
    Task<(bool Exito, string Mensaje)> AnularAsync(int requerimientoId);
    Task<bool> HayItemsConStockBajoAsync();
}

public interface IEmpleadoService
{
    Task<IEnumerable<Empleado>> GetTodosAsync(bool soloActivos = true);
    Task<Empleado?> GetPorIdAsync(int id);
    Task<Empleado?> GetPorDNIAsync(string dni);
    Task<(bool Exito, string Mensaje, Empleado? Empleado)> CrearAsync(Empleado empleado);
    Task<(bool Exito, string Mensaje)> ActualizarAsync(Empleado empleado);
    Task<(bool Exito, string Mensaje)> ActivarDesactivarAsync(int empleadoId);
    Task<string> GenerarCodigoAsync();
}
