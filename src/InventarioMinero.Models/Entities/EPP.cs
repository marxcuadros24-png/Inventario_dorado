using InventarioMinero.Models.Enums;

namespace InventarioMinero.Models.Entities;

public class EPP
{
    public int EPPId { get; set; }
    public string Codigo { get; set; } = string.Empty;       // EPP-001
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CategoriaId { get; set; }
    public int UnidadId { get; set; }
    public string? Marca { get; set; }
    public string? Talla { get; set; }                       // S, M, L, 40, 41...
    public int? VidaUtilMeses { get; set; }
    public decimal StockMinimo { get; set; }
    public decimal? StockMaximo { get; set; }
    public decimal StockActual { get; set; }
    public decimal PrecioUnitario { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public int? CreadoPor { get; set; }

    // Navegación
    public Categoria Categoria { get; set; } = null!;
    public UnidadMedida Unidad { get; set; } = null!;
    public ICollection<DetalleEntregaEPP> Entregas { get; set; } = new List<DetalleEntregaEPP>();
    public ICollection<DetalleRequerimiento> DetallesRequerimiento { get; set; } = new List<DetalleRequerimiento>();

    public override string ToString() => Nombre;
}

public class EntregaEPP
{
    public int EntregaId { get; set; }
    public string Numero { get; set; } = string.Empty;       // ENT-EPP-2024-000001
    public int EmpleadoId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public MotivoEntregaEPP Motivo { get; set; } = MotivoEntregaEPP.Dotacion;
    public bool FirmaEmpleado { get; set; }
    public string? Observaciones { get; set; }
    public bool Anulado { get; set; }

    // Navegación
    public Empleado Empleado { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public ICollection<DetalleEntregaEPP> Detalles { get; set; } = new List<DetalleEntregaEPP>();
}

public class DetalleEntregaEPP
{
    public int DetalleEntregaId { get; set; }
    public int EntregaId { get; set; }
    public int EPPId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string? Observaciones { get; set; }

    // Computed
    public decimal Total => Cantidad * PrecioUnitario;

    // Navegación
    public EntregaEPP EntregaEPP { get; set; } = null!;
    public EPP EPP { get; set; } = null!;
}
