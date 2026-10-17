namespace InventarioMinero.Models.Enums;

public enum EstadoAsignacion
{
    Activo,
    Devuelto,
    Perdido
}

public enum CondicionHerramienta
{
    Bueno,
    Regular,
    Deteriorado,
    Perdido
}

public enum EstadoMovimiento
{
    Borrador,
    Confirmado,
    Anulado
}

public enum TipoMovimientoCodigo
{
    ENT,
    SAL,
    AJU,
    DEV
}

public enum EstadoRequerimiento
{
    Pendiente,
    Atendido,
    Anulado
}

public enum MotivoEntregaEPP
{
    Dotacion,
    Reposicion,
    Deterioro,
    Perdida
}

public enum TipoItem
{
    Herramienta,
    EPP
}

public enum TipoCategoria
{
    Herramienta,
    EPP,
    Ambos
}

public enum EstadoStock
{
    Normal,
    Bajo,
    Minimo,
    SinStock
}

public enum RolSistema
{
    Administrador = 1,
    Almacenero = 2,
    Supervisor = 3
}
