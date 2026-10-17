using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.DAL.Context;

public class InventarioDbContext : DbContext
{
    public InventarioDbContext(DbContextOptions<InventarioDbContext> options) : base(options) { }

    // ── DbSets ────────────────────────────────────────────────
    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<UnidadMedida> UnidadesMedida => Set<UnidadMedida>();
    public DbSet<Herramienta> Herramientas => Set<Herramienta>();
    public DbSet<StockHerramienta> StocksHerramienta => Set<StockHerramienta>();
    public DbSet<TipoMovimiento> TiposMovimiento => Set<TipoMovimiento>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();
    public DbSet<DetalleMovimiento> DetallesMovimiento => Set<DetalleMovimiento>();
    public DbSet<AsignacionHerramienta> AsignacionesHerramienta => Set<AsignacionHerramienta>();
    public DbSet<EPP> EPPs => Set<EPP>();
    public DbSet<EntregaEPP> EntregasEPP => Set<EntregaEPP>();
    public DbSet<DetalleEntregaEPP> DetallesEntregaEPP => Set<DetalleEntregaEPP>();
    public DbSet<Requerimiento> Requerimientos => Set<Requerimiento>();
    public DbSet<DetalleRequerimiento> DetallesRequerimiento => Set<DetalleRequerimiento>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // ── Cargo ──────────────────────────────────────────────
        mb.Entity<Cargo>(e =>
        {
            e.ToTable("Cargo");
            e.HasKey(x => x.CargoId);
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
            e.HasIndex(x => x.Nombre).IsUnique();
        });

        // ── Empleado ───────────────────────────────────────────
        mb.Entity<Empleado>(e =>
        {
            e.ToTable("Empleado");
            e.HasKey(x => x.EmpleadoId);
            e.Property(x => x.Codigo).IsRequired().HasMaxLength(20);
            e.Property(x => x.Nombres).IsRequired().HasMaxLength(100);
            e.Property(x => x.Apellidos).IsRequired().HasMaxLength(100);
            e.Property(x => x.DNI).IsRequired().HasMaxLength(15);
            e.Property(x => x.Area).HasMaxLength(100);
            e.HasIndex(x => x.Codigo).IsUnique();
            e.HasIndex(x => x.DNI).IsUnique();
            e.Ignore(x => x.NombreCompleto);
            e.HasOne(x => x.Cargo).WithMany(c => c.Empleados)
             .HasForeignKey(x => x.CargoId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── Rol ────────────────────────────────────────────────
        mb.Entity<Rol>(e =>
        {
            e.ToTable("Rol");
            e.HasKey(x => x.RolId);
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Nombre).IsUnique();
        });

        // ── Usuario ────────────────────────────────────────────
        mb.Entity<Usuario>(e =>
        {
            e.ToTable("Usuario");
            e.HasKey(x => x.UsuarioId);
            e.Property(x => x.NombreUsuario).IsRequired().HasMaxLength(50);
            e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);
            e.HasIndex(x => x.NombreUsuario).IsUnique();
            e.HasIndex(x => x.EmpleadoId).IsUnique();
            e.HasOne(x => x.Empleado).WithOne(emp => emp.Usuario)
             .HasForeignKey<Usuario>(x => x.EmpleadoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Rol).WithMany(r => r.Usuarios)
             .HasForeignKey(x => x.RolId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── Categoria ──────────────────────────────────────────
        mb.Entity<Categoria>(e =>
        {
            e.ToTable("Categoria");
            e.HasKey(x => x.CategoriaId);
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
            e.Property(x => x.Tipo).HasConversion<string>().HasMaxLength(15);
            e.HasIndex(x => x.Nombre).IsUnique();
        });

        // ── UnidadMedida ───────────────────────────────────────
        mb.Entity<UnidadMedida>(e =>
        {
            e.ToTable("UnidadMedida");
            e.HasKey(x => x.UnidadId);
            e.Property(x => x.Codigo).IsRequired().HasMaxLength(10);
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Codigo).IsUnique();
        });

        // ── Herramienta ────────────────────────────────────────
        mb.Entity<Herramienta>(e =>
        {
            e.ToTable("Herramienta");
            e.HasKey(x => x.HerramientaId);
            e.Property(x => x.Codigo).IsRequired().HasMaxLength(30);
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(200);
            e.Property(x => x.StockMinimo).HasPrecision(12, 3);
            e.Property(x => x.StockMaximo).HasPrecision(12, 3);
            e.Property(x => x.PrecioUnitario).HasPrecision(12, 2);
            e.HasIndex(x => x.Codigo).IsUnique();
            e.HasOne(x => x.Categoria).WithMany(c => c.Herramientas)
             .HasForeignKey(x => x.CategoriaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Unidad).WithMany(u => u.Herramientas)
             .HasForeignKey(x => x.UnidadId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── StockHerramienta ───────────────────────────────────
        mb.Entity<StockHerramienta>(e =>
        {
            e.ToTable("StockHerramienta");
            e.HasKey(x => x.StockId);
            e.Property(x => x.CantidadActual).HasPrecision(12, 3);
            e.Property(x => x.CantidadReservada).HasPrecision(12, 3);
            e.Ignore(x => x.CantidadDisponible);
            e.HasIndex(x => x.HerramientaId).IsUnique();
            e.HasOne(x => x.Herramienta).WithOne(h => h.Stock)
             .HasForeignKey<StockHerramienta>(x => x.HerramientaId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── TipoMovimiento ─────────────────────────────────────
        mb.Entity<TipoMovimiento>(e =>
        {
            e.ToTable("TipoMovimiento");
            e.HasKey(x => x.TipoMovId);
            e.Property(x => x.Codigo).IsRequired().HasMaxLength(10);
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Codigo).IsUnique();
        });

        // ── Movimiento ─────────────────────────────────────────
        mb.Entity<Movimiento>(e =>
        {
            e.ToTable("Movimiento");
            e.HasKey(x => x.MovimientoId);
            e.Property(x => x.Numero).IsRequired().HasMaxLength(20);
            e.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(x => x.Numero).IsUnique();
            e.HasOne(x => x.TipoMovimiento).WithMany(t => t.Movimientos)
             .HasForeignKey(x => x.TipoMovId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Usuario).WithMany(u => u.Movimientos)
             .HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Requerimiento).WithMany()
             .HasForeignKey(x => x.RequerimientoId).OnDelete(DeleteBehavior.SetNull)
             .IsRequired(false);
        });

        // ── DetalleMovimiento ──────────────────────────────────
        mb.Entity<DetalleMovimiento>(e =>
        {
            e.ToTable("DetalleMovimiento");
            e.HasKey(x => x.DetalleMovId);
            e.Property(x => x.Cantidad).HasPrecision(12, 3);
            e.Property(x => x.PrecioUnitario).HasPrecision(12, 2);
            e.Ignore(x => x.Total);
            e.HasOne(x => x.Movimiento).WithMany(m => m.Detalles)
             .HasForeignKey(x => x.MovimientoId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Herramienta).WithMany(h => h.DetallesMovimiento)
             .HasForeignKey(x => x.HerramientaId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── AsignacionHerramienta ──────────────────────────────
        mb.Entity<AsignacionHerramienta>(e =>
        {
            e.ToTable("AsignacionHerramienta");
            e.HasKey(x => x.AsignacionId);
            e.Property(x => x.Numero).IsRequired().HasMaxLength(20);
            e.Property(x => x.Cantidad).HasPrecision(12, 3);
            e.Property(x => x.CondicionSalida).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.CondicionRetorno).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(x => x.Numero).IsUnique();
            e.HasOne(x => x.Herramienta).WithMany(h => h.Asignaciones)
             .HasForeignKey(x => x.HerramientaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Empleado).WithMany(emp => emp.Asignaciones)
             .HasForeignKey(x => x.EmpleadoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Usuario).WithMany(u => u.Asignaciones)
             .HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── EPP ────────────────────────────────────────────────
        mb.Entity<EPP>(e =>
        {
            e.ToTable("EPP");
            e.HasKey(x => x.EPPId);
            e.Property(x => x.Codigo).IsRequired().HasMaxLength(30);
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(200);
            e.Property(x => x.StockMinimo).HasPrecision(12, 3);
            e.Property(x => x.StockMaximo).HasPrecision(12, 3);
            e.Property(x => x.StockActual).HasPrecision(12, 3);
            e.Property(x => x.PrecioUnitario).HasPrecision(12, 2);
            e.HasIndex(x => x.Codigo).IsUnique();
            e.HasOne(x => x.Categoria).WithMany(c => c.EPPs)
             .HasForeignKey(x => x.CategoriaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Unidad).WithMany(u => u.EPPs)
             .HasForeignKey(x => x.UnidadId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── EntregaEPP ─────────────────────────────────────────
        mb.Entity<EntregaEPP>(e =>
        {
            e.ToTable("EntregaEPP");
            e.HasKey(x => x.EntregaId);
            e.Property(x => x.Numero).IsRequired().HasMaxLength(20);
            e.Property(x => x.Motivo).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(x => x.Numero).IsUnique();
            e.HasOne(x => x.Empleado).WithMany(emp => emp.EntregasEPP)
             .HasForeignKey(x => x.EmpleadoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Usuario).WithMany(u => u.EntregasEPP)
             .HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── DetalleEntregaEPP ──────────────────────────────────
        mb.Entity<DetalleEntregaEPP>(e =>
        {
            e.ToTable("DetalleEntregaEPP");
            e.HasKey(x => x.DetalleEntregaId);
            e.Property(x => x.Cantidad).HasPrecision(12, 3);
            e.Property(x => x.PrecioUnitario).HasPrecision(12, 2);
            e.Ignore(x => x.Total);
            e.HasOne(x => x.EntregaEPP).WithMany(ent => ent.Detalles)
             .HasForeignKey(x => x.EntregaId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.EPP).WithMany(ep => ep.Entregas)
             .HasForeignKey(x => x.EPPId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── Requerimiento ──────────────────────────────────────
        mb.Entity<Requerimiento>(e =>
        {
            e.ToTable("Requerimiento");
            e.HasKey(x => x.RequerimientoId);
            e.Property(x => x.Numero).IsRequired().HasMaxLength(20);
            e.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(x => x.Numero).IsUnique();
            e.HasOne(x => x.Usuario).WithMany()
             .HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Movimiento).WithMany()
             .HasForeignKey(x => x.MovimientoId).OnDelete(DeleteBehavior.SetNull)
             .IsRequired(false);
        });

        // ── DetalleRequerimiento ───────────────────────────────
        mb.Entity<DetalleRequerimiento>(e =>
        {
            e.ToTable("DetalleRequerimiento");
            e.HasKey(x => x.DetalleReqId);
            e.Property(x => x.TipoItem).HasConversion<string>().HasMaxLength(15);
            e.Property(x => x.StockActual).HasPrecision(12, 3);
            e.Property(x => x.StockMinimo).HasPrecision(12, 3);
            e.Property(x => x.CantidadSugerida).HasPrecision(12, 3);
            e.Property(x => x.CantidadAprobada).HasPrecision(12, 3);
            e.Ignore(x => x.ItemNombre);
            e.Ignore(x => x.ItemCodigo);
            e.Ignore(x => x.ItemUnidad);
            e.HasOne(x => x.Requerimiento).WithMany(r => r.Detalles)
             .HasForeignKey(x => x.RequerimientoId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Herramienta).WithMany(h => h.DetallesRequerimiento)
             .HasForeignKey(x => x.HerramientaId).OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);
            e.HasOne(x => x.EPP).WithMany(ep => ep.DetallesRequerimiento)
             .HasForeignKey(x => x.EPPId).OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);
        });

        // ── Seed Data ──────────────────────────────────────────
        mb.Entity<Rol>().HasData(
            new Rol { RolId = 1, Nombre = "Administrador", Descripcion = "Acceso total al sistema" },
            new Rol { RolId = 2, Nombre = "Almacenero",    Descripcion = "Gestión de entradas, salidas y EPP" },
            new Rol { RolId = 3, Nombre = "Supervisor",    Descripcion = "Aprobación y reportes" }
        );

        mb.Entity<TipoMovimiento>().HasData(
            new TipoMovimiento { TipoMovId = 1, Codigo = "ENT", Nombre = "Entrada por Compra",   EsEntrada = true  },
            new TipoMovimiento { TipoMovId = 2, Codigo = "SAL", Nombre = "Salida por Consumo",   EsEntrada = false },
            new TipoMovimiento { TipoMovId = 3, Codigo = "AJU", Nombre = "Ajuste de Inventario", EsEntrada = true  },
            new TipoMovimiento { TipoMovId = 4, Codigo = "DEV", Nombre = "Devolución a Almacén", EsEntrada = true  }
        );

        mb.Entity<UnidadMedida>().HasData(
            new UnidadMedida { UnidadId = 1, Codigo = "UND", Nombre = "Unidad" },
            new UnidadMedida { UnidadId = 2, Codigo = "PAR", Nombre = "Par" },
            new UnidadMedida { UnidadId = 3, Codigo = "JGO", Nombre = "Juego" },
            new UnidadMedida { UnidadId = 4, Codigo = "MT",  Nombre = "Metro" },
            new UnidadMedida { UnidadId = 5, Codigo = "KG",  Nombre = "Kilogramo" },
            new UnidadMedida { UnidadId = 6, Codigo = "CJA", Nombre = "Caja" }
        );

        mb.Entity<Categoria>().HasData(
            new Categoria { CategoriaId = 1,  Nombre = "Protección de Cabeza",        Tipo = TipoCategoria.EPP },
            new Categoria { CategoriaId = 2,  Nombre = "Protección Visual",           Tipo = TipoCategoria.EPP },
            new Categoria { CategoriaId = 3,  Nombre = "Protección Auditiva",         Tipo = TipoCategoria.EPP },
            new Categoria { CategoriaId = 4,  Nombre = "Protección Respiratoria",     Tipo = TipoCategoria.EPP },
            new Categoria { CategoriaId = 5,  Nombre = "Protección de Manos",         Tipo = TipoCategoria.EPP },
            new Categoria { CategoriaId = 6,  Nombre = "Protección de Pies",          Tipo = TipoCategoria.EPP },
            new Categoria { CategoriaId = 7,  Nombre = "Ropa de Trabajo",             Tipo = TipoCategoria.EPP },
            new Categoria { CategoriaId = 8,  Nombre = "Herramientas Manuales",       Tipo = TipoCategoria.Herramienta },
            new Categoria { CategoriaId = 9,  Nombre = "Herramientas Eléctricas",     Tipo = TipoCategoria.Herramienta },
            new Categoria { CategoriaId = 10, Nombre = "Herramientas de Medición",    Tipo = TipoCategoria.Herramienta },
            new Categoria { CategoriaId = 11, Nombre = "Equipos de Izaje",            Tipo = TipoCategoria.Herramienta },
            new Categoria { CategoriaId = 12, Nombre = "Instrumentos de Perforación", Tipo = TipoCategoria.Herramienta }
        );

        mb.Entity<Cargo>().HasData(
            new Cargo { CargoId = 1, Nombre = "Jefe de Almacén" },
            new Cargo { CargoId = 2, Nombre = "Almacenero" },
            new Cargo { CargoId = 3, Nombre = "Supervisor de Mina" },
            new Cargo { CargoId = 4, Nombre = "Operador de Equipo" },
            new Cargo { CargoId = 5, Nombre = "Técnico de Mantenimiento" },
            new Cargo { CargoId = 6, Nombre = "Gerente de Operaciones" }
        );
    }
}
