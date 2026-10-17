using InventarioMinero.Models.Enums;

namespace InventarioMinero.Models.Entities;

public class Categoria
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoCategoria Tipo { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Herramienta> Herramientas { get; set; } = new List<Herramienta>();
    public ICollection<EPP> EPPs { get; set; } = new List<EPP>();
}

public class UnidadMedida
{
    public int UnidadId { get; set; }
    public string Codigo { get; set; } = string.Empty;   // UND, PAR, JGO
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public ICollection<Herramienta> Herramientas { get; set; } = new List<Herramienta>();
    public ICollection<EPP> EPPs { get; set; } = new List<EPP>();
}
