using InventarioMinero.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventarioMinero.WPF.Views;

public partial class AsignacionesView : UserControl
{
    private AsignacionesViewModel? ViewModel => DataContext as AsignacionesViewModel;

    public AsignacionesView()
    {
        InitializeComponent();
    }

    public AsignacionesView(AsignacionesViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Inicialización gestionada centralmente por MainViewModel.NavegarAsync
    }

    private void Busqueda_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && ViewModel?.CargarAsignacionesCommand.CanExecute(null) == true)
        {
            ViewModel.CargarAsignacionesCommand.Execute(null);
        }
    }
}
