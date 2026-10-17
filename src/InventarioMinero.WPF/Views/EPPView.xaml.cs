using InventarioMinero.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventarioMinero.WPF.Views;

public partial class EPPView : UserControl
{
    private EPPViewModel? ViewModel => DataContext as EPPViewModel;

    public EPPView()
    {
        InitializeComponent();
    }

    public EPPView(EPPViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Inicialización gestionada centralmente por MainViewModel.NavegarAsync
    }

    private void Busqueda_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && ViewModel?.CargarEPPCommand.CanExecute(null) == true)
        {
            ViewModel.CargarEPPCommand.Execute(null);
        }
    }
}
