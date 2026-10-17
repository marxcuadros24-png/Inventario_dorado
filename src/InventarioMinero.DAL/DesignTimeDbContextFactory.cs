using InventarioMinero.DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace InventarioMinero.DAL;

/// <summary>
/// Factory usado por dotnet-ef en tiempo de diseño para crear el DbContext
/// sin necesidad de arrancar la aplicación WPF.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<InventarioDbContext>
{
    public InventarioDbContext CreateDbContext(string[] args)
    {
        // Busca appsettings.json en el proyecto WPF (relativo a DAL)
        var basePath = Path.Combine(Directory.GetCurrentDirectory(),
            "..", "InventarioMinero.WPF");

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connStr = config.GetConnectionString("InventarioDB");

        var optionsBuilder = new DbContextOptionsBuilder<InventarioDbContext>();
        optionsBuilder.UseSqlServer(connStr);

        return new InventarioDbContext(optionsBuilder.Options);
    }
}
