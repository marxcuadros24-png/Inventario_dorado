using InventarioMinero.Models.Entities;
using InventarioMinero.WPF.ViewModels;
using System.Windows;

namespace InventarioMinero.WPF;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    public void ConfigurarUsuario(Usuario usuario)
    {
        _viewModel.Inicializar(usuario);
    }
}