using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace InventarioMinero.WPF.ViewModels;

public partial class AsignacionesViewModel : ObservableObject
{
    private readonly IAsignacionService _asignacionService;
    private readonly IHerramientaService _herramientaService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IStockService _stockService;

    [ObservableProperty]
    private ObservableCollection<AsignacionHerramienta> _asignaciones = new();

    [ObservableProperty]
    private ObservableCollection<Herramienta> _herramientas = new();

    [ObservableProperty]
    private ObservableCollection<Empleado> _empleados = new();

    [ObservableProperty]
    private ObservableCollection<string> _condiciones = new() { "Bueno", "Regular", "Deteriorado", "Perdido" };

    [ObservableProperty]
    private string _busqueda = string.Empty;

    [ObservableProperty]
    private bool _soloActivas = true;

    [ObservableProperty]
    private bool _soloVencidas = false;

    [ObservableProperty]
    private AsignacionHerramienta? _asignacionSeleccionada;

    // KPIs
    [ObservableProperty]
    private int _kpiPrestamosActivos;

    [ObservableProperty]
    private int _kpiPrestamosVencidos;

    [ObservableProperty]
    private int _kpiDevueltosHoy;

    [ObservableProperty]
    private int _kpiTrabajadoresConPrestamos;

    // Modales
    [ObservableProperty]
    private bool _mostrarModalNuevo;

    [ObservableProperty]
    private bool _mostrarModalDevolucion;

    // Formulario Nuevo Préstamo
    [ObservableProperty]
    private Empleado? _nuevoEmpleado;

    [ObservableProperty]
    private Herramienta? _nuevaHerramienta;

    [ObservableProperty]
    private decimal _nuevaCantidad = 1;

    [ObservableProperty]
    private DateTime _nuevaFechaDevolucion = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private string? _nuevasObservaciones;

    [ObservableProperty]
    private decimal _stockDisponibleActual;

    // Formulario Devolución
    [ObservableProperty]
    private AsignacionHerramienta? _asignacionADevolver;

    [ObservableProperty]
    private string _condicionRetorno = "Bueno";

    [ObservableProperty]
    private string? _observacionesDevolucion;

    // Alertas y estado
    [ObservableProperty]
    private string? _mensajeBanner;

    [ObservableProperty]
    private bool _esErrorBanner;

    [ObservableProperty]
    private bool _estaCargando;

    public int UsuarioActualId { get; set; } = 1;

    public AsignacionesViewModel(
        IAsignacionService asignacionService,
        IHerramientaService herramientaService,
        IEmpleadoService empleadoService,
        IStockService stockService)
    {
        _asignacionService = asignacionService;
        _herramientaService = herramientaService;
        _empleadoService = empleadoService;
        _stockService = stockService;
    }

    private readonly SemaphoreSlim _initLock = new(1, 1);

    public async Task InicializarAsync()
    {
        if (!await _initLock.WaitAsync(0)) return;
        try
        {
            MensajeBanner = null;
            await CargarCatalogosAsync();
            await CargarAsignacionesAsync();
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task CargarCatalogosAsync()
    {
        try
        {
            var hList = await _herramientaService.GetTodasAsync(soloActivas: true);
            Herramientas = new ObservableCollection<Herramienta>(hList);

            var eList = await _empleadoService.GetTodosAsync(soloActivos: true);
            Empleados = new ObservableCollection<Empleado>(eList);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error cargando catálogos: {ex.Message}", true);
        }
    }

    [RelayCommand]
    public async Task CargarAsignacionesAsync()
    {
        EstaCargando = true;
        try
        {
            var lista = await _asignacionService.GetActivasAsync();
            var todas = lista.ToList();

            // Filtrado en memoria
            var filtradas = todas.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                var term = Busqueda.Trim().ToLower();
                filtradas = filtradas.Where(a =>
                    a.Numero.ToLower().Contains(term) ||
                    a.Herramienta.Nombre.ToLower().Contains(term) ||
                    a.Herramienta.Codigo.ToLower().Contains(term) ||
                    a.Empleado.Nombres.ToLower().Contains(term) ||
                    a.Empleado.Apellidos.ToLower().Contains(term) ||
                    a.Empleado.DNI.Contains(term));
            }

            if (SoloVencidas)
            {
                filtradas = filtradas.Where(a => a.FechaDevProgramada.HasValue && a.FechaDevProgramada.Value.Date < DateTime.Today);
            }

            Asignaciones = new ObservableCollection<AsignacionHerramienta>(filtradas);

            // Calcular KPIs
            KpiPrestamosActivos = todas.Count(a => a.Estado == EstadoAsignacion.Activo);
            KpiPrestamosVencidos = todas.Count(a => a.Estado == EstadoAsignacion.Activo && a.FechaDevProgramada.HasValue && a.FechaDevProgramada.Value.Date < DateTime.Today);
            KpiDevueltosHoy = todas.Count(a => a.FechaDevReal.HasValue && a.FechaDevReal.Value.Date == DateTime.Today);
            KpiTrabajadoresConPrestamos = todas.Where(a => a.Estado == EstadoAsignacion.Activo).Select(a => a.EmpleadoId).Distinct().Count();
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al cargar préstamos: {ex.Message}", true);
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public void AbrirModalNuevo()
    {
        NuevoEmpleado = Empleados.FirstOrDefault();
        NuevaHerramienta = Herramientas.FirstOrDefault();
        NuevaCantidad = 1;
        NuevaFechaDevolucion = DateTime.Today.AddDays(1);
        NuevasObservaciones = string.Empty;
        ActualizarStockDisponible();
        MostrarModalNuevo = true;
    }

    [RelayCommand]
    public void CerrarModalNuevo()
    {
        MostrarModalNuevo = false;
    }

    partial void OnNuevaHerramientaChanged(Herramienta? value)
    {
        ActualizarStockDisponible();
    }

    private void ActualizarStockDisponible()
    {
        if (NuevaHerramienta?.Stock != null)
        {
            StockDisponibleActual = NuevaHerramienta.Stock.CantidadDisponible;
        }
        else
        {
            StockDisponibleActual = 0;
        }
    }

    [RelayCommand]
    public async Task GuardarNuevoPrestamoAsync()
    {
        if (NuevoEmpleado == null)
        {
            MostrarMensaje("Debe seleccionar al trabajador minero responsable.", true);
            return;
        }

        if (NuevaHerramienta == null)
        {
            MostrarMensaje("Debe seleccionar la herramienta a prestar.", true);
            return;
        }

        if (NuevaCantidad <= 0)
        {
            MostrarMensaje("La cantidad a prestar debe ser mayor a cero.", true);
            return;
        }

        if (NuevaCantidad > StockDisponibleActual)
        {
            MostrarMensaje($"No hay suficiente stock disponible. Disponible: {StockDisponibleActual:N2}, Solicitado: {NuevaCantidad:N2}", true);
            return;
        }

        EstaCargando = true;
        try
        {
            var res = await _asignacionService.AsignarAsync(
                NuevaHerramienta.HerramientaId,
                NuevoEmpleado.EmpleadoId,
                NuevaCantidad,
                UsuarioActualId,
                NuevaFechaDevolucion,
                NuevasObservaciones);

            if (res.Exito)
            {
                MostrarModalNuevo = false;
                MostrarMensaje(res.Mensaje, false);
                await CargarCatalogosAsync();
                await CargarAsignacionesAsync();
            }
            else
            {
                MostrarMensaje(res.Mensaje, true);
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error inesperado: {ex.Message}", true);
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public void AbrirModalDevolucion(AsignacionHerramienta? asignacion)
    {
        var target = asignacion ?? AsignacionSeleccionada;
        if (target == null)
        {
            MostrarMensaje("Seleccione un préstamo para registrar su devolución.", true);
            return;
        }

        AsignacionADevolver = target;
        CondicionRetorno = "Bueno";
        ObservacionesDevolucion = string.Empty;
        MostrarModalDevolucion = true;
    }

    [RelayCommand]
    public void CerrarModalDevolucion()
    {
        MostrarModalDevolucion = false;
        AsignacionADevolver = null;
    }

    [RelayCommand]
    public async Task ProcesarDevolucionAsync()
    {
        if (AsignacionADevolver == null) return;

        EstaCargando = true;
        try
        {
            var res = await _asignacionService.RegistrarDevolucionAsync(
                AsignacionADevolver.AsignacionId,
                CondicionRetorno,
                ObservacionesDevolucion);

            if (res.Exito)
            {
                MostrarModalDevolucion = false;
                AsignacionADevolver = null;
                MostrarMensaje(res.Mensaje, false);
                await CargarCatalogosAsync();
                await CargarAsignacionesAsync();
            }
            else
            {
                MostrarMensaje(res.Mensaje, true);
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al procesar devolución: {ex.Message}", true);
        }
        finally
        {
            EstaCargando = false;
        }
    }

    private void MostrarMensaje(string mensaje, bool esError)
    {
        MensajeBanner = mensaje;
        EsErrorBanner = esError;

        _ = Task.Run(async () =>
        {
            await Task.Delay(5000);
            if (MensajeBanner == mensaje)
            {
                MensajeBanner = null;
            }
        });
    }
}
