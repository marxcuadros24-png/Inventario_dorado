using InventarioMinero.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventarioMinero.WPF.Views;

public partial class MovimientosView : UserControl
{
    private MovimientosViewModel? ViewModel => DataContext as MovimientosViewModel;

    public MovimientosView()
    {
        InitializeComponent();
    }

    public MovimientosView(MovimientosViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Inicialización gestionada centralmente por MainViewModel.NavegarAsync
    }

    private void Busqueda_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && ViewModel?.CargarMovimientosCommand.CanExecute(null) == true)
        {
            ViewModel.CargarMovimientosCommand.Execute(null);
        }
    }
}
