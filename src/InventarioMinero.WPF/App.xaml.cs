using InventarioMinero.BLL.Services.Implementations;
using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.DAL;
using InventarioMinero.DAL.Context;
using InventarioMinero.WPF.ViewModels;
using InventarioMinero.WPF.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;

namespace InventarioMinero.WPF;

public partial class App : Application
{
    private IHost _host = null!;

    private static void Log(string msg)
    {
        try
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup.log");
            File.AppendAllText(path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {msg}{Environment.NewLine}");
        }
        catch { }
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Log("OnStartup iniciado");
        DispatcherUnhandledException += (s, args) =>
        {
            Log($"DispatcherUnhandledException: {args.Exception}");
        };
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            Log($"UnhandledException: {args.ExceptionObject}");
        };

        // Prevenir que WPF cierre la aplicación al ceder el hilo durante operaciones asíncronas iniciales
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        try
        {
            Log("Creando Host...");
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(cfg =>
                {
                    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    cfg.SetBasePath(baseDir)
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((ctx, services) =>
                {
                    var connStr = ctx.Configuration.GetConnectionString("InventarioDB");

                    // ── DAL ──────────────────────────────────────
                    services.AddDbContext<InventarioDbContext>(opt =>
                        opt.UseSqlServer(connStr), ServiceLifetime.Transient, ServiceLifetime.Transient);

                    // ── BLL ──────────────────────────────────────
                    services.AddTransient<IUsuarioService, UsuarioService>();
                    services.AddTransient<ICatalogoService, CatalogoService>();
                    services.AddTransient<IHerramientaService, HerramientaService>();
                    services.AddTransient<IStockService, StockService>();
                    services.AddTransient<IMovimientoService, MovimientoService>();
                    services.AddTransient<IEPPService, EPPService>();
                    services.AddTransient<IRequerimientoService, RequerimientoService>();
                    services.AddTransient<IEmpleadoService, EmpleadoService>();
                    services.AddTransient<IAsignacionService, AsignacionService>();
                    services.AddTransient<IEntregaEPPService, EntregaEPPService>();

                    // ── WPF ViewModels & Views ───────────────────
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<LoginWindow>();
                    services.AddSingleton<HerramientasViewModel>();
                    services.AddTransient<HerramientasView>();
                    services.AddSingleton<EPPViewModel>();
                    services.AddTransient<EPPView>();
                    services.AddSingleton<RequerimientosViewModel>();
                    services.AddTransient<RequerimientosView>();
                    services.AddSingleton<MovimientosViewModel>();
                    services.AddTransient<MovimientosView>();
                    services.AddSingleton<AsignacionesViewModel>();
                    services.AddTransient<AsignacionesView>();
                    services.AddSingleton<EntregasEPPViewModel>();
                    services.AddTransient<EntregasEPPView>();
                    services.AddSingleton<EmpleadosViewModel>();
                    services.AddTransient<EmpleadosView>();
                    services.AddSingleton<UsuariosViewModel>();
                    services.AddTransient<UsuariosView>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();

            Log("Iniciando host.StartAsync()...");
            await _host.StartAsync();
            Log("Host iniciado con éxito.");

            // ── INICIALIZACIÓN Y SEEDING DE BASE DE DATOS ─────────
            Log("Verificando DbInitializer...");
            using (var scope = _host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();
                await DbInitializer.InitializeAsync(db);
            }
            Log("DbInitializer completado.");

            // ── FLUJO DE AUTENTICACIÓN: LOGIN WINDOW ─────────────
            Log("Creando LoginWindow...");
            var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
            var loginVm = (LoginViewModel)loginWindow.DataContext;
            Log("Mostrando LoginWindow (ShowDialog)...");

            bool? loginResult = loginWindow.ShowDialog();
            Log($"LoginWindow cerrado con resultado: {loginResult}");

            if (loginResult == true && loginVm.UsuarioAutenticado != null)
            {
                Log("Login exitoso. Creando MainWindow...");
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.ConfigurarUsuario(loginVm.UsuarioAutenticado);
                MainWindow = mainWindow;
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                mainWindow.Show();
                Log("MainWindow mostrada.");
            }
            else
            {
                Log("Login cancelado o cerrado. Finalizando aplicación.");
                Shutdown();
            }
        }
        catch (Exception ex)
        {
            Log($"EXCEPCIÓN CRÍTICA EN ONSTARTUP: {ex}");
            MessageBox.Show(
                $"Error crítico al iniciar la aplicación:\n\n{ex.Message}\n\nDetalles:\n{ex.InnerException?.Message}",
                "Error en Planta Minera Dorado S.A.",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        base.OnExit(e);
    }

    public static T GetService<T>() where T : class
    {
        var app = (App)Current;
        return app._host.Services.GetRequiredService<T>();
    }
}
