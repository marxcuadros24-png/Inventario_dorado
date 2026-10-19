namespace InventarioMinero.Models.Entities;

public class Rol
{
    public int RolId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    public override string ToString() => Nombre;
}

public class Usuario
{
    public int UsuarioId { get; set; }
    public int EmpleadoId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int RolId { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime? UltimoAcceso { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public int? CreadoPor { get; set; }

    // Navegación
    public Empleado Empleado { get; set; } = null!;
    public Rol Rol { get; set; } = null!;
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    public ICollection<AsignacionHerramienta> Asignaciones { get; set; } = new List<AsignacionHerramienta>();
    public ICollection<EntregaEPP> EntregasEPP { get; set; } = new List<EntregaEPP>();

    public override string ToString() => NombreUsuario;
}
