using InventarioMinero.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventarioMinero.WPF.Views;

public partial class RequerimientosView : UserControl
{
    private RequerimientosViewModel? ViewModel => DataContext as RequerimientosViewModel;

    public RequerimientosView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    public RequerimientosView(RequerimientosViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is RequerimientosViewModel oldVm)
        {
            oldVm.RequestPrintVisual -= OnRequestPrintVisual;
        }
        if (e.NewValue is RequerimientosViewModel newVm)
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
        if (e.Key == Key.Enter && ViewModel?.CargarRequerimientosCommand.CanExecute(null) == true)
        {
            ViewModel.CargarRequerimientosCommand.Execute(null);
        }
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.ImprimirHojaCommand.Execute(HojaImprimible);
    }

    private void OnRequestPrintVisual(object? sender, FrameworkElement visualToPrint)
    {
        var printDlg = new PrintDialog();
        if (printDlg.ShowDialog() == true)
        {
            printDlg.PrintVisual(visualToPrint, $"Requerimiento_{ViewModel?.RequerimientoParaImprimir?.Numero ?? "Compra"}");
        }
    }
}
