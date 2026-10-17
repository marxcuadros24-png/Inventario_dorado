namespace InventarioMinero.Models.Entities;

public class StockHerramienta
{
    public int StockId { get; set; }
    public int HerramientaId { get; set; }
    public decimal CantidadActual { get; set; }
    public decimal CantidadReservada { get; set; }    // en asignaciones activas
    public DateTime UltimaActualizacion { get; set; } = DateTime.Now;

    // Computed
    public decimal CantidadDisponible => CantidadActual - CantidadReservada;

    // Navegación
    public Herramienta Herramienta { get; set; } = null!;
}
