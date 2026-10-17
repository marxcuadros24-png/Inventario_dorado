using InventarioMinero.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventarioMinero.WPF.Views;

public partial class EmpleadosView : UserControl
{
    private EmpleadosViewModel? ViewModel => DataContext as EmpleadosViewModel;

    public EmpleadosView()
    {
        InitializeComponent();
    }

    public EmpleadosView(EmpleadosViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Inicialización gestionada centralmente por MainViewModel.NavegarAsync
    }

    private void Busqueda_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && ViewModel?.CargarEmpleadosCommand.CanExecute(null) == true)
        {
            ViewModel.CargarEmpleadosCommand.Execute(null);
        }
    }
}
