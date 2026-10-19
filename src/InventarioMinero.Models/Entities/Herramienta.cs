namespace InventarioMinero.Models.Entities;

public class Herramienta
{
    public int HerramientaId { get; set; }
    public string Codigo { get; set; } = string.Empty;       // HTM-001
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CategoriaId { get; set; }
    public int UnidadId { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public decimal StockMinimo { get; set; }
    public decimal? StockMaximo { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string? Imagen { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public int? CreadoPor { get; set; }

    // Navegación
    public Categoria Categoria { get; set; } = null!;
    public UnidadMedida Unidad { get; set; } = null!;
    public StockHerramienta? Stock { get; set; }
    public ICollection<DetalleMovimiento> DetallesMovimiento { get; set; } = new List<DetalleMovimiento>();
    public ICollection<AsignacionHerramienta> Asignaciones { get; set; } = new List<AsignacionHerramienta>();
    public ICollection<DetalleRequerimiento> DetallesRequerimiento { get; set; } = new List<DetalleRequerimiento>();

    public override string ToString() => Nombre;
}
