using InventarioMinero.Models.Enums;

namespace InventarioMinero.Models.Entities;

public class AsignacionHerramienta
{
    public int AsignacionId { get; set; }
    public string Numero { get; set; } = string.Empty;       // ASG-2024-000001
    public int HerramientaId { get; set; }
    public int EmpleadoId { get; set; }
    public decimal Cantidad { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    public DateTime? FechaDevProgramada { get; set; }
    public DateTime? FechaDevReal { get; set; }
    public CondicionHerramienta CondicionSalida { get; set; } = CondicionHerramienta.Bueno;
    public CondicionHerramienta? CondicionRetorno { get; set; }
    public int UsuarioId { get; set; }
    public string? Observaciones { get; set; }
    public EstadoAsignacion Estado { get; set; } = EstadoAsignacion.Activo;

    // Navegación
    public Herramienta Herramienta { get; set; } = null!;
    public Empleado Empleado { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
