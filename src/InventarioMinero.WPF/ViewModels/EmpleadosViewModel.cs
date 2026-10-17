using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.Models.Entities;
using System.Collections.ObjectModel;

namespace InventarioMinero.WPF.ViewModels;

public partial class EmpleadosViewModel : ObservableObject
{
    private readonly IEmpleadoService _empleadoService;
    private readonly ICatalogoService _catalogoService;

    [ObservableProperty]
    private ObservableCollection<Empleado> _empleados = new();

    [ObservableProperty]
    private ObservableCollection<Cargo> _cargos = new();

    [ObservableProperty]
    private Empleado? _empleadoSeleccionado;

    // Filtros
    [ObservableProperty]
    private string _busqueda = string.Empty;

    [ObservableProperty]
    private Cargo? _cargoFiltro;

    [ObservableProperty]
    private bool _soloActivos = true;

    // KPIs
    [ObservableProperty]
    private int _kpiTotalPersonal;

    [ObservableProperty]
    private int _kpiPersonalActivo;

    [ObservableProperty]
    private int _kpiTotalCargos;

    // Modal
    [ObservableProperty]
    private bool _mostrarModal;

    [ObservableProperty]
    private bool _esEdicion;

    // Campos del formulario
    [ObservableProperty]
    private int _formId;

    [ObservableProperty]
    private string _formCodigo = string.Empty;

    [ObservableProperty]
    private string _formNombres = string.Empty;

    [ObservableProperty]
    private string _formApellidos = string.Empty;

    [ObservableProperty]
    private string _formDNI = string.Empty;

    [ObservableProperty]
    private Cargo? _formCargo;

    [ObservableProperty]
    private string _formArea = "Operaciones Mina";

    [ObservableProperty]
    private string _formTelefono = string.Empty;

    [ObservableProperty]
    private string _formEmail = string.Empty;

    // Alertas
    [ObservableProperty]
    private string? _mensajeBanner;

    [ObservableProperty]
    private bool _esErrorBanner;

    [ObservableProperty]
    private bool _estaCargando;

    public EmpleadosViewModel(IEmpleadoService empleadoService, ICatalogoService catalogoService)
    {
        _empleadoService = empleadoService;
        _catalogoService = catalogoService;
    }

    private readonly SemaphoreSlim _initLock = new(1, 1);

    public async Task InicializarAsync()
    {
        if (!await _initLock.WaitAsync(0)) return;
        try
        {
            MensajeBanner = null;
            await CargarCargosAsync();
            await CargarEmpleadosAsync();
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task CargarCargosAsync()
    {
        try
        {
            var list = await _catalogoService.GetCargosAsync();
            Cargos = new ObservableCollection<Cargo>(list);
            KpiTotalCargos = Cargos.Count;
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error cargando cargos: {ex.Message}", true);
        }
    }

    [RelayCommand]
    public async Task CargarEmpleadosAsync()
    {
        EstaCargando = true;
        try
        {
            var list = await _empleadoService.GetTodosAsync(soloActivos: false);
            var todas = list.ToList();

            var filtradas = todas.AsEnumerable();
            if (SoloActivos)
                filtradas = filtradas.Where(e => e.Activo);

            if (CargoFiltro != null)
                filtradas = filtradas.Where(e => e.CargoId == CargoFiltro.CargoId);

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                var term = Busqueda.Trim().ToLower();
                filtradas = filtradas.Where(e =>
                    e.Codigo.ToLower().Contains(term) ||
                    e.NombreCompleto.ToLower().Contains(term) ||
                    e.DNI.Contains(term) ||
                    (e.Area != null && e.Area.ToLower().Contains(term)));
            }

            Empleados = new ObservableCollection<Empleado>(filtradas);

            // KPIs
            KpiTotalPersonal = todas.Count;
            KpiPersonalActivo = todas.Count(e => e.Activo);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al cargar empleados: {ex.Message}", true);
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public async Task AbrirModalNuevoAsync()
    {
        EsEdicion = false;
        FormId = 0;
        FormCodigo = await _empleadoService.GenerarCodigoAsync();
        FormNombres = string.Empty;
        FormApellidos = string.Empty;
        FormDNI = string.Empty;
        FormCargo = Cargos.FirstOrDefault();
        FormArea = "Operaciones Mina";
        FormTelefono = string.Empty;
        FormEmail = string.Empty;

        MostrarModal = true;
    }

    [RelayCommand]
    public void AbrirModalEditar(Empleado? empleado)
    {
        var target = empleado ?? EmpleadoSeleccionado;
        if (target == null) return;

        EsEdicion = true;
        FormId = target.EmpleadoId;
        FormCodigo = target.Codigo;
        FormNombres = target.Nombres;
        FormApellidos = target.Apellidos;
        FormDNI = target.DNI;
        FormCargo = Cargos.FirstOrDefault(c => c.CargoId == target.CargoId);
        FormArea = target.Area ?? string.Empty;
        FormTelefono = target.Telefono ?? string.Empty;
        FormEmail = target.Email ?? string.Empty;

        MostrarModal = true;
    }

    [RelayCommand]
    public void CerrarModal()
    {
        MostrarModal = false;
    }

    [RelayCommand]
    public async Task GuardarEmpleadoAsync()
    {
        if (string.IsNullOrWhiteSpace(FormNombres) || string.IsNullOrWhiteSpace(FormApellidos))
        {
            MostrarMensaje("Debe ingresar nombres y apellidos del trabajador.", true);
            return;
        }

        if (string.IsNullOrWhiteSpace(FormDNI) || FormDNI.Trim().Length != 8)
        {
            MostrarMensaje("El DNI debe contener exactamente 8 dígitos numéricos.", true);
            return;
        }

        if (FormCargo == null)
        {
            MostrarMensaje("Debe seleccionar un cargo u ocupación en la planta minera.", true);
            return;
        }

        EstaCargando = true;
        try
        {
            if (EsEdicion)
            {
                var emp = new Empleado
                {
                    EmpleadoId = FormId,
                    Codigo = FormCodigo,
                    Nombres = FormNombres.Trim(),
                    Apellidos = FormApellidos.Trim(),
                    DNI = FormDNI.Trim(),
                    CargoId = FormCargo.CargoId,
                    Area = FormArea,
                    Telefono = FormTelefono,
                    Email = FormEmail,
                    Activo = true
                };

                var res = await _empleadoService.ActualizarAsync(emp);
                if (res.Exito)
                {
                    MostrarModal = false;
                    MostrarMensaje(res.Mensaje, false);
                    await CargarEmpleadosAsync();
                }
                else
                {
                    MostrarMensaje(res.Mensaje, true);
                }
            }
            else
            {
                var emp = new Empleado
                {
                    Codigo = FormCodigo,
                    Nombres = FormNombres.Trim(),
                    Apellidos = FormApellidos.Trim(),
                    DNI = FormDNI.Trim(),
                    CargoId = FormCargo.CargoId,
                    Area = FormArea,
                    Telefono = FormTelefono,
                    Email = FormEmail,
                    FechaIngreso = DateTime.Today,
                    Activo = true
                };

                var res = await _empleadoService.CrearAsync(emp);
                if (res.Exito)
                {
                    MostrarModal = false;
                    MostrarMensaje(res.Mensaje, false);
                    await CargarEmpleadosAsync();
                }
                else
                {
                    MostrarMensaje(res.Mensaje, true);
                }
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al guardar empleado: {ex.Message}", true);
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public async Task AlternarEstadoAsync(Empleado? empleado)
    {
        if (empleado == null) return;

        EstaCargando = true;
        try
        {
            var res = await _empleadoService.ActivarDesactivarAsync(empleado.EmpleadoId);
            MostrarMensaje(res.Mensaje, !res.Exito);
            await CargarEmpleadosAsync();
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error: {ex.Message}", true);
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
