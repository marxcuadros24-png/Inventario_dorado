using InventarioMinero.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventarioMinero.WPF.Views;

public partial class UsuariosView : UserControl
{
    private UsuariosViewModel? ViewModel => DataContext as UsuariosViewModel;

    public UsuariosView()
    {
        InitializeComponent();
    }

    public UsuariosView(UsuariosViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Inicialización gestionada centralmente por MainViewModel.NavegarAsync
    }

    private void Busqueda_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && ViewModel?.CargarUsuariosCommand.CanExecute(null) == true)
        {
            ViewModel.CargarUsuariosCommand.Execute(null);
        }
    }
}
