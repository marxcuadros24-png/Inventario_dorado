using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.Models.Entities;
using System.Windows;

namespace InventarioMinero.WPF.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUsuarioService _usuarioService;

    [ObservableProperty]
    private string _usuario = "admin";

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string? _mensajeError;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _recordarUsuario = true;

    [ObservableProperty]
    private string _estadoServidor = "Planta Minera LAN • SQL Server Activo";

    public Usuario? UsuarioAutenticado { get; private set; }

    // Evento disparado para que la vista cierre o transicione
    public event EventHandler<Usuario>? LoginExitoso;
    public event EventHandler? RequestClose;

    public LoginViewModel(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [RelayCommand]
    private async Task IniciarSesionAsync()
    {
        if (string.IsNullOrWhiteSpace(Usuario))
        {
            MensajeError = "Ingrese su nombre de usuario.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            MensajeError = "Ingrese su contraseña de acceso.";
            return;
        }

        try
        {
            IsBusy = true;
            MensajeError = null;

            // Pequeña pausa para feedback visual de conexión
            await Task.Delay(300);

            var (exito, usuario, mensaje) = await _usuarioService.AutenticarAsync(Usuario.Trim(), Password);

            if (!exito || usuario is null)
            {
                MensajeError = mensaje;
                return;
            }

            UsuarioAutenticado = usuario;
            LoginExitoso?.Invoke(this, usuario);
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al conectar con la base de datos: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cerrar(Window? window)
    {
        window?.Close();
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Minimizar(Window? window)
    {
        if (window != null)
        {
            window.WindowState = WindowState.Minimized;
        }
    }
}
