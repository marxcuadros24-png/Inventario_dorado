namespace InventarioMinero.Models.Entities;

public class Empleado
{
    public int EmpleadoId { get; set; }
    public string Codigo { get; set; } = string.Empty;      // EMP-001
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public int CargoId { get; set; }
    public string? Area { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public DateTime FechaIngreso { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Navegación
    public Cargo Cargo { get; set; } = null!;
    public Usuario? Usuario { get; set; }
    public ICollection<AsignacionHerramienta> Asignaciones { get; set; } = new List<AsignacionHerramienta>();
    public ICollection<EntregaEPP> EntregasEPP { get; set; } = new List<EntregaEPP>();
    public ICollection<Requerimiento> Requerimientos { get; set; } = new List<Requerimiento>();

    // Computed (with empty setter to avoid WPF TwoWay binding crashes)
    public string NombreCompleto
    {
        get => $"{Nombres} {Apellidos}".Trim();
        set { }
    }

    public override string ToString() => NombreCompleto;
}
