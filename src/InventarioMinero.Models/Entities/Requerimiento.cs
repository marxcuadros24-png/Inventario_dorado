using InventarioMinero.Models.Enums;

namespace InventarioMinero.Models.Entities;

public class Requerimiento
{
    public int RequerimientoId { get; set; }
    public string Numero { get; set; } = string.Empty;       // REQ-2024-000001
    public DateTime Fecha { get; set; } = DateTime.Now;
    public int UsuarioId { get; set; }
    public string? Area { get; set; }
    public string? Observaciones { get; set; }
    public EstadoRequerimiento Estado { get; set; } = EstadoRequerimiento.Pendiente;
    public DateTime? FechaImpresion { get; set; }            // última vez que se imprimió
    public DateTime? FechaAtencion { get; set; }
    public int? MovimientoId { get; set; }                   // mov. de entrada que lo atiende

    // Navegación
    public Usuario Usuario { get; set; } = null!;
    public Movimiento? Movimiento { get; set; }
    public ICollection<DetalleRequerimiento> Detalles { get; set; } = new List<DetalleRequerimiento>();
}

public class DetalleRequerimiento
{
    public int DetalleReqId { get; set; }
    public int RequerimientoId { get; set; }
    public TipoItem TipoItem { get; set; }
    public int? HerramientaId { get; set; }
    public int? EPPId { get; set; }
    public decimal StockActual { get; set; }                 // captura al momento de generación
    public decimal StockMinimo { get; set; }                 // captura al momento de generación
    public decimal CantidadSugerida { get; set; }            // StockMax - StockActual
    public decimal? CantidadAprobada { get; set; }           // el admin puede ajustar
    public string? Observaciones { get; set; }

    // Navegación
    public Requerimiento Requerimiento { get; set; } = null!;
    public Herramienta? Herramienta { get; set; }
    public EPP? EPP { get; set; }

    // Computed (with empty setters to avoid WPF TwoWay binding crashes)
    public string ItemNombre
    {
        get => TipoItem == TipoItem.Herramienta ? Herramienta?.Nombre ?? "-" : EPP?.Nombre ?? "-";
        set { }
    }

    public string ItemCodigo
    {
        get => TipoItem == TipoItem.Herramienta ? Herramienta?.Codigo ?? "-" : EPP?.Codigo ?? "-";
        set { }
    }

    public string ItemUnidad
    {
        get => TipoItem == TipoItem.Herramienta ? Herramienta?.Unidad?.Codigo ?? "UND" : EPP?.Unidad?.Codigo ?? "UND";
        set { }
    }
}
