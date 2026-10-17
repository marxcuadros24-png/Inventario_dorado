using InventarioMinero.WPF.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace InventarioMinero.WPF.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        _viewModel.RequestClose += (s, e) => Close();
        _viewModel.LoginExitoso += (s, usuario) =>
        {
            DialogResult = true;
            Close();
        };
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && _viewModel.IniciarSesionCommand.CanExecute(null))
        {
            _viewModel.IniciarSesionCommand.Execute(null);
        }
    }
}
