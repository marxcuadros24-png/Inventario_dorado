using InventarioMinero.Models.Enums;

namespace InventarioMinero.Models.Entities;

public class TipoMovimiento
{
    public int TipoMovId { get; set; }
    public string Codigo { get; set; } = string.Empty;   // ENT, SAL, AJU, DEV
    public string Nombre { get; set; } = string.Empty;
    public bool EsEntrada { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}

public class Movimiento
{
    public int MovimientoId { get; set; }
    public string Numero { get; set; } = string.Empty;      // MOV-2024-000001
    public int TipoMovId { get; set; }
    public int? RequerimientoId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public int UsuarioId { get; set; }
    public string? DocumentoRef { get; set; }               // Nro. factura/guía
    public string? Observaciones { get; set; }
    public EstadoMovimiento Estado { get; set; } = EstadoMovimiento.Confirmado;
    public DateTime? FechaAnulacion { get; set; }
    public int? AnuladoPor { get; set; }

    // Navegación
    public TipoMovimiento TipoMovimiento { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public Requerimiento? Requerimiento { get; set; }
    public ICollection<DetalleMovimiento> Detalles { get; set; } = new List<DetalleMovimiento>();
}

public class DetalleMovimiento
{
    public int DetalleMovId { get; set; }
    public int MovimientoId { get; set; }
    public int HerramientaId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string? Observaciones { get; set; }

    // Computed
    public decimal Total => Cantidad * PrecioUnitario;

    // Navegación
    public Movimiento Movimiento { get; set; } = null!;
    public Herramienta Herramienta { get; set; } = null!;
}
