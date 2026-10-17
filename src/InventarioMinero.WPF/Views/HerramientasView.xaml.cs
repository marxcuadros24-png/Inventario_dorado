using InventarioMinero.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventarioMinero.WPF.Views;

public partial class HerramientasView : UserControl
{
    private HerramientasViewModel? ViewModel => DataContext as HerramientasViewModel;

    public HerramientasView()
    {
        InitializeComponent();
    }

    public HerramientasView(HerramientasViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Inicialización gestionada centralmente por MainViewModel.NavegarAsync
    }

    private void Busqueda_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && ViewModel?.CargarHerramientasCommand.CanExecute(null) == true)
        {
            ViewModel.CargarHerramientasCommand.Execute(null);
        }
    }
}
