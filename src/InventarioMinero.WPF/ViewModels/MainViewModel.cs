using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioMinero.Models.Entities;
using System.Windows;

namespace InventarioMinero.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly HerramientasViewModel _herramientasViewModel;
    private readonly EPPViewModel _eppViewModel;
    private readonly RequerimientosViewModel _requerimientosViewModel;
    private readonly MovimientosViewModel _movimientosViewModel;
    private readonly AsignacionesViewModel _asignacionesViewModel;
    private readonly EntregasEPPViewModel _entregasEPPViewModel;
    private readonly EmpleadosViewModel _empleadosViewModel;
    private readonly UsuariosViewModel _usuariosViewModel;

    [ObservableProperty]
    private Usuario? _usuarioActual;

    [ObservableProperty]
    private string _nombreCompleto = "Administrador Planta";

    [ObservableProperty]
    private string _rolNombre = "Administrador";

    [ObservableProperty]
    private string _terminalNombre = "PC-TERMINAL-01 (LAN)";

    [ObservableProperty]
    private string _seccionActual = "Dashboard";

    [ObservableProperty]
    private object? _currentView;

    public HerramientasViewModel HerramientasVm => _herramientasViewModel;
    public EPPViewModel EPPVm => _eppViewModel;
    public RequerimientosViewModel RequerimientosVm => _requerimientosViewModel;
    public MovimientosViewModel MovimientosVm => _movimientosViewModel;
    public AsignacionesViewModel AsignacionesVm => _asignacionesViewModel;
    public EntregasEPPViewModel EntregasEPPVm => _entregasEPPViewModel;
    public EmpleadosViewModel EmpleadosVm => _empleadosViewModel;
    public UsuariosViewModel UsuariosVm => _usuariosViewModel;

    public MainViewModel(
        HerramientasViewModel herramientasViewModel, 
        EPPViewModel eppViewModel,
        RequerimientosViewModel requerimientosViewModel,
        MovimientosViewModel movimientosViewModel,
        AsignacionesViewModel asignacionesViewModel,
        EntregasEPPViewModel entregasEPPViewModel,
        EmpleadosViewModel empleadosViewModel,
        UsuariosViewModel usuariosViewModel)
    {
        _herramientasViewModel = herramientasViewModel;
        _eppViewModel = eppViewModel;
        _requerimientosViewModel = requerimientosViewModel;
        _movimientosViewModel = movimientosViewModel;
        _asignacionesViewModel = asignacionesViewModel;
        _entregasEPPViewModel = entregasEPPViewModel;
        _empleadosViewModel = empleadosViewModel;
        _usuariosViewModel = usuariosViewModel;
    }

    public void Inicializar(Usuario usuario)
    {
        UsuarioActual = usuario;
        NombreCompleto = usuario.Empleado != null 
            ? $"{usuario.Empleado.Nombres} {usuario.Empleado.Apellidos}"
            : usuario.NombreUsuario;
        RolNombre = usuario.Rol?.Nombre ?? "Usuario";

        _movimientosViewModel.UsuarioActualId = usuario.UsuarioId;
        _asignacionesViewModel.UsuarioActualId = usuario.UsuarioId;
        _entregasEPPViewModel.UsuarioActualId = usuario.UsuarioId;
    }

    [RelayCommand]
    public async Task NavegarAsync(string destino)
    {
        SeccionActual = destino;

        if (destino == "Herramientas")
        {
            CurrentView = _herramientasViewModel;
            await _herramientasViewModel.InicializarAsync();
        }
        else if (destino == "EPP")
        {
            CurrentView = _eppViewModel;
            await _eppViewModel.InicializarAsync();
        }
        else if (destino == "Requerimientos")
        {
            CurrentView = _requerimientosViewModel;
            await _requerimientosViewModel.InicializarAsync();
        }
        else if (destino == "Movimientos")
        {
            CurrentView = _movimientosViewModel;
            await _movimientosViewModel.InicializarAsync();
        }
        else if (destino == "Asignaciones")
        {
            CurrentView = _asignacionesViewModel;
            await _asignacionesViewModel.InicializarAsync();
        }
        else if (destino == "EntregasEPP")
        {
            CurrentView = _entregasEPPViewModel;
            await _entregasEPPViewModel.InicializarAsync();
        }
        else if (destino == "Empleados")
        {
            CurrentView = _empleadosViewModel;
            await _empleadosViewModel.InicializarAsync();
        }
        else if (destino == "Usuarios")
        {
            CurrentView = _usuariosViewModel;
            await _usuariosViewModel.InicializarAsync();
        }
        else
        {
            CurrentView = null; // Dashboard
        }
    }

    [RelayCommand]
    private void CerrarSesion(Window? window)
    {
        var result = MessageBox.Show(
            "¿Desea cerrar la sesión actual?",
            "Confirmación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            System.Diagnostics.Process.Start(Environment.ProcessPath!);
            Application.Current.Shutdown();
        }
    }
}
