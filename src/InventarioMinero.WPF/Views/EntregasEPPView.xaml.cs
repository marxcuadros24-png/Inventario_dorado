using InventarioMinero.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventarioMinero.WPF.Views;

public partial class EntregasEPPView : UserControl
{
    private EntregasEPPViewModel? ViewModel => DataContext as EntregasEPPViewModel;

    public EntregasEPPView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    public EntregasEPPView(EntregasEPPViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is EntregasEPPViewModel oldVm)
        {
            oldVm.RequestPrintVisual -= OnRequestPrintVisual;
        }
        if (e.NewValue is EntregasEPPViewModel newVm)
        {
            newVm.RequestPrintVisual += OnRequestPrintVisual;
        }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Inicialización gestionada centralmente por MainViewModel.NavegarAsync
    }

    private void Busqueda_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && ViewModel?.CargarEntregasCommand.CanExecute(null) == true)
        {
            ViewModel.CargarEntregasCommand.Execute(null);
        }
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.ImprimirCargoCommand.Execute(HojaCargoImprimible);
    }

    private void OnRequestPrintVisual(object? sender, FrameworkElement visualToPrint)
    {
        var printDlg = new PrintDialog();
        if (printDlg.ShowDialog() == true)
        {
            printDlg.PrintVisual(visualToPrint, $"Cargo_EPP_{ViewModel?.EntregaSeleccionada?.Numero ?? "Dotacion"}");
        }
    }
}
