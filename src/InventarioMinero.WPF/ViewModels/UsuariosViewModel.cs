using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.Models.Entities;
using System.Collections.ObjectModel;

namespace InventarioMinero.WPF.ViewModels;

public partial class UsuariosViewModel : ObservableObject
{
    private readonly IUsuarioService _usuarioService;
    private readonly IEmpleadoService _empleadoService;
    private readonly ICatalogoService _catalogoService;

    [ObservableProperty]
    private ObservableCollection<Usuario> _usuarios = new();

    [ObservableProperty]
    private ObservableCollection<Empleado> _empleados = new();

    [ObservableProperty]
    private ObservableCollection<Rol> _roles = new();

    [ObservableProperty]
    private Usuario? _usuarioSeleccionado;

    // Filtros
    [ObservableProperty]
    private string _busqueda = string.Empty;

    // KPIs
    [ObservableProperty]
    private int _kpiTotalUsuarios;

    [ObservableProperty]
    private int _kpiUsuariosActivos;

    [ObservableProperty]
    private int _kpiAdministradores;

    // Modales
    [ObservableProperty]
    private bool _mostrarModalNuevo;

    [ObservableProperty]
    private bool _mostrarModalPassword;

    // Formulario Nuevo Usuario
    [ObservableProperty]
    private Empleado? _nuevoEmpleado;

    [ObservableProperty]
    private Rol? _nuevoRol;

    [ObservableProperty]
    private string _nuevoUsername = string.Empty;

    [ObservableProperty]
    private string _nuevoPassword = string.Empty;

    // Formulario Cambio Password
    [ObservableProperty]
    private Usuario? _usuarioPasswordTarget;

    [ObservableProperty]
    private string _nuevoPasswordReset = string.Empty;

    // Alertas
    [ObservableProperty]
    private string? _mensajeBanner;

    [ObservableProperty]
    private bool _esErrorBanner;

    [ObservableProperty]
    private bool _estaCargando;

    public UsuariosViewModel(
        IUsuarioService usuarioService,
        IEmpleadoService empleadoService,
        ICatalogoService catalogoService)
    {
        _usuarioService = usuarioService;
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
            await CargarCatalogosAsync();
            await CargarUsuariosAsync();
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
            var empList = await _empleadoService.GetTodosAsync(soloActivos: true);
            Empleados = new ObservableCollection<Empleado>(empList);

            var rolesList = await _catalogoService.GetRolesAsync();
            Roles = new ObservableCollection<Rol>(rolesList);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al cargar catálogos: {ex.Message}", true);
        }
    }

    [RelayCommand]
    public async Task CargarUsuariosAsync()
    {
        EstaCargando = true;
        try
        {
            var list = await _usuarioService.GetTodosAsync();
            var todas = list.ToList();

            var filtradas = todas.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                var term = Busqueda.Trim().ToLower();
                filtradas = filtradas.Where(u =>
                    u.NombreUsuario.ToLower().Contains(term) ||
                    (u.Empleado != null && u.Empleado.NombreCompleto.ToLower().Contains(term)) ||
                    (u.Rol != null && u.Rol.Nombre.ToLower().Contains(term)));
            }

            Usuarios = new ObservableCollection<Usuario>(filtradas);

            // KPIs
            KpiTotalUsuarios = todas.Count;
            KpiUsuariosActivos = todas.Count(u => u.Activo);
            KpiAdministradores = todas.Count(u => u.RolId == 1 && u.Activo);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al cargar usuarios: {ex.Message}", true);
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
        NuevoRol = Roles.FirstOrDefault(r => r.RolId == 2) ?? Roles.FirstOrDefault();
        NuevoUsername = string.Empty;
        NuevoPassword = string.Empty;
        MostrarModalNuevo = true;
    }

    [RelayCommand]
    public void CerrarModalNuevo()
    {
        MostrarModalNuevo = false;
    }

    [RelayCommand]
    public async Task GuardarNuevoUsuarioAsync()
    {
        if (NuevoEmpleado == null)
        {
            MostrarMensaje("Debe asociar la cuenta a un trabajador de la planta.", true);
            return;
        }

        if (string.IsNullOrWhiteSpace(NuevoUsername))
        {
            MostrarMensaje("Ingrese un nombre de usuario.", true);
            return;
        }

        if (string.IsNullOrWhiteSpace(NuevoPassword) || NuevoPassword.Length < 4)
        {
            MostrarMensaje("La contraseña debe tener al menos 4 caracteres.", true);
            return;
        }

        if (NuevoRol == null)
        {
            MostrarMensaje("Debe asignar un rol en el sistema.", true);
            return;
        }

        EstaCargando = true;
        try
        {
            var user = new Usuario
            {
                NombreUsuario = NuevoUsername.Trim(),
                EmpleadoId = NuevoEmpleado.EmpleadoId,
                RolId = NuevoRol.RolId,
                Activo = true
            };

            var res = await _usuarioService.CrearAsync(user, NuevoPassword);
            if (res.Exito)
            {
                MostrarModalNuevo = false;
                MostrarMensaje(res.Mensaje, false);
                await CargarUsuariosAsync();
            }
            else
            {
                MostrarMensaje(res.Mensaje, true);
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al crear usuario: {ex.Message}", true);
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public void AbrirModalPassword(Usuario? usuario)
    {
        var target = usuario ?? UsuarioSeleccionado;
        if (target == null) return;

        UsuarioPasswordTarget = target;
        NuevoPasswordReset = string.Empty;
        MostrarModalPassword = true;
    }

    [RelayCommand]
    public void CerrarModalPassword()
    {
        MostrarModalPassword = false;
        UsuarioPasswordTarget = null;
    }

    [RelayCommand]
    public async Task GuardarCambioPasswordAsync()
    {
        if (UsuarioPasswordTarget == null) return;

        if (string.IsNullOrWhiteSpace(NuevoPasswordReset) || NuevoPasswordReset.Length < 4)
        {
            MostrarMensaje("La nueva contraseña debe tener al menos 4 caracteres.", true);
            return;
        }

        EstaCargando = true;
        try
        {
            var res = await _usuarioService.CambiarPasswordAsync(
                UsuarioPasswordTarget.UsuarioId,
                string.Empty, // bypass o reset administrativo
                NuevoPasswordReset);

            if (res.Exito)
            {
                MostrarModalPassword = false;
                UsuarioPasswordTarget = null;
                MostrarMensaje(res.Mensaje, false);
            }
            else
            {
                MostrarMensaje(res.Mensaje, true);
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al cambiar clave: {ex.Message}", true);
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public async Task AlternarEstadoAsync(Usuario? usuario)
    {
        if (usuario == null) return;

        EstaCargando = true;
        try
        {
            var res = await _usuarioService.ActivarDesactivarAsync(usuario.UsuarioId);
            MostrarMensaje(res.Mensaje, !res.Exito);
            await CargarUsuariosAsync();
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
