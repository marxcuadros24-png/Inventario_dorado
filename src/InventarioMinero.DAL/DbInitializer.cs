using InventarioMinero.DAL.Context;
using InventarioMinero.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.DAL;

public static class DbInitializer
{
    public static async Task InitializeAsync(InventarioDbContext context)
    {
        await context.Database.MigrateAsync();

        // 1. Usuarios y Empleados iniciales
        if (!await context.Usuarios.AnyAsync())
        {
            var adminEmpleado = new Empleado
            {
                Codigo = "EMP-001",
                Nombres = "Administrador",
                Apellidos = "Planta Minera",
                DNI = "00000000",
                CargoId = 1, // Jefe de Almacén
                Area = "Almacén Central y Planta",
                FechaIngreso = DateTime.Now.AddYears(-1),
                Activo = true
            };
            await context.Empleados.AddAsync(adminEmpleado);
            await context.SaveChangesAsync();

            var adminUsuario = new Usuario
            {
                EmpleadoId = adminEmpleado.EmpleadoId,
                NombreUsuario = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123", 11),
                RolId = 1, // Administrador
                Activo = true,
                FechaCreacion = DateTime.Now
            };
            await context.Usuarios.AddAsync(adminUsuario);

            var almaceneroEmpleado = new Empleado
            {
                Codigo = "EMP-002",
                Nombres = "Carlos",
                Apellidos = "Mendoza Roca",
                DNI = "12345678",
                CargoId = 2, // Almacenero
                Area = "Operaciones Mina",
                FechaIngreso = DateTime.Now.AddMonths(-6),
                Activo = true
            };
            await context.Empleados.AddAsync(almaceneroEmpleado);
            await context.SaveChangesAsync();

            var almaceneroUsuario = new Usuario
            {
                EmpleadoId = almaceneroEmpleado.EmpleadoId,
                NombreUsuario = "almacen",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("almacen123", 11),
                RolId = 2, // Almacenero
                Activo = true,
                FechaCreacion = DateTime.Now
            };
            await context.Usuarios.AddAsync(almaceneroUsuario);
            await context.SaveChangesAsync();
        }

        // 2. Muestra de EPP
        if (!await context.EPPs.AnyAsync())
        {
            var epps = new List<EPP>
            {
                new EPP { Codigo = "EPP-001", Nombre = "Casco de Seguridad Minero Clase E Blanco", CategoriaId = 1, UnidadId = 1, StockActual = 45, StockMinimo = 15, VidaUtilMeses = 12, PrecioUnitario = 45.00m, Activo = true },
                new EPP { Codigo = "EPP-002", Nombre = "Lentes de Seguridad Anti-empañante Oscuros", CategoriaId = 2, UnidadId = 1, StockActual = 8, StockMinimo = 20, VidaUtilMeses = 2, PrecioUnitario = 18.50m, Activo = true }, // Stock bajo para requerimiento
                new EPP { Codigo = "EPP-003", Nombre = "Tapones Auditivos Silicona Reutilizables 27dB", CategoriaId = 3, UnidadId = 2, StockActual = 80, StockMinimo = 30, VidaUtilMeses = 1, PrecioUnitario = 4.20m, Activo = true },
                new EPP { Codigo = "EPP-004", Nombre = "Respirador de Media Cara Serie 6200 c/ Filtros 2097", CategoriaId = 4, UnidadId = 1, StockActual = 12, StockMinimo = 10, VidaUtilMeses = 6, Talla = "M", PrecioUnitario = 85.00m, Activo = true },
                new EPP { Codigo = "EPP-005", Nombre = "Guantes de Cuero Badana para Mecánico Minero", CategoriaId = 5, UnidadId = 2, StockActual = 5, StockMinimo = 25, VidaUtilMeses = 1, Talla = "L", PrecioUnitario = 15.00m, Activo = true }, // Stock bajo
                new EPP { Codigo = "EPP-006", Nombre = "Botas de Jebe con Puntera de Acero y Plantilla", CategoriaId = 6, UnidadId = 2, StockActual = 22, StockMinimo = 10, VidaUtilMeses = 6, Talla = "42", PrecioUnitario = 120.00m, Activo = true }
            };
            await context.EPPs.AddRangeAsync(epps);
            await context.SaveChangesAsync();
        }

        // 3. Muestra de Herramientas
        if (!await context.Herramientas.AnyAsync())
        {
            var h1 = new Herramienta { Codigo = "HTM-001", Nombre = "Llave Francesa Stilson 18 Pulgadas", CategoriaId = 8, UnidadId = 1, Marca = "Stanley FatMax", Modelo = "Pro-18", StockMinimo = 5, PrecioUnitario = 85.00m, Activo = true };
            var h2 = new Herramienta { Codigo = "HTM-002", Nombre = "Taladro Percutor Inalámbrico 20V MAX", CategoriaId = 9, UnidadId = 1, Marca = "DeWalt", Modelo = "DCD996", StockMinimo = 2, PrecioUnitario = 850.00m, Activo = true };
            var h3 = new Herramienta { Codigo = "HTM-003", Nombre = "Multímetro Digital True RMS Industrial", CategoriaId = 10, UnidadId = 1, Marca = "Fluke", Modelo = "Fluke 87V", StockMinimo = 1, PrecioUnitario = 1450.00m, Activo = true };
            var h4 = new Herramienta { Codigo = "HTM-004", Nombre = "Eslinga de Poliéster 3 Pulgadas x 6 Metros (5T)", CategoriaId = 11, UnidadId = 1, Marca = "Yale", Modelo = "ESL-5T-6M", StockMinimo = 4, PrecioUnitario = 180.00m, Activo = true };

            await context.Herramientas.AddRangeAsync(h1, h2, h3, h4);
            await context.SaveChangesAsync();

            var stocks = new List<StockHerramienta>
            {
                new StockHerramienta { HerramientaId = h1.HerramientaId, CantidadActual = 15, CantidadReservada = 3, UltimaActualizacion = DateTime.Now },
                new StockHerramienta { HerramientaId = h2.HerramientaId, CantidadActual = 4, CantidadReservada = 1, UltimaActualizacion = DateTime.Now },
                new StockHerramienta { HerramientaId = h3.HerramientaId, CantidadActual = 3, CantidadReservada = 1, UltimaActualizacion = DateTime.Now },
                new StockHerramienta { HerramientaId = h4.HerramientaId, CantidadActual = 8, CantidadReservada = 0, UltimaActualizacion = DateTime.Now }
            };
            await context.StocksHerramienta.AddRangeAsync(stocks);
            await context.SaveChangesAsync();
        }
    }
}
