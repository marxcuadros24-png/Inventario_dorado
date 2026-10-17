namespace InventarioMinero.Models.Entities;

public class Cargo
{
    public int CargoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Navegación
    public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
