using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace InventarioMinero.WPF.ViewModels;

public partial class HerramientasViewModel : ObservableObject
{
    private readonly IHerramientaService _herramientaService;
    private readonly ICatalogoService _catalogoService;

    public ObservableCollection<Herramienta> Herramientas { get; } = new();
    public ObservableCollection<Categoria> Categorias { get; } = new();
    public ObservableCollection<UnidadMedida> Unidades { get; } = new();

    // Filtros
    [ObservableProperty]
    private string _busqueda = string.Empty;

    [ObservableProperty]
    private Categoria? _categoriaFiltro;

    [ObservableProperty]
    private bool _soloBajoStock;

    // Estado UI
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private Herramienta? _herramientaSeleccionada;

    [ObservableProperty]
    private bool _isFormOpen;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private bool _isCreateMode = true;

    [ObservableProperty]
    private string? _mensajeFeedback;

    [ObservableProperty]
    private bool _isFeedbackSuccess;

    // Campos del Formulario
    [ObservableProperty]
    private int _formId;

    [ObservableProperty]
    private string _formCodigo = string.Empty;

    [ObservableProperty]
    private string _formNombre = string.Empty;

    [ObservableProperty]
    private string? _formDescripcion;

    [ObservableProperty]
    private string? _formMarca;

    [ObservableProperty]
    private string? _formModelo;

    [ObservableProperty]
    private int _formCategoriaId;

    [ObservableProperty]
    private int _formUnidadId;

    [ObservableProperty]
    private decimal _formStockMinimo = 2;

    [ObservableProperty]
    private decimal? _formStockMaximo;

    [ObservableProperty]
    private decimal _formPrecioUnitario = 0;

    [ObservableProperty]
    private decimal _formStockInicial = 0;

    [ObservableProperty]
    private bool _formActivo = true;

    // KPIs en tiempo real
    [ObservableProperty]
    private int _totalHerramientas;

    [ObservableProperty]
    private decimal _totalStockUnidades;

    [ObservableProperty]
    private int _alertasStockBajo;

    public HerramientasViewModel(IHerramientaService herramientaService, ICatalogoService catalogoService)
    {
        _herramientaService = herramientaService;
        _catalogoService = catalogoService;
    }

    private readonly SemaphoreSlim _initLock = new(1, 1);

    [RelayCommand]
    public async Task InicializarAsync()
    {
        if (!await _initLock.WaitAsync(0)) return;
        try
        {
            MensajeFeedback = null;
            await CargarCatalogosAsync();
            await CargarHerramientasAsync();
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task CargarCatalogosAsync()
    {
        try
        {
            var cats = await _catalogoService.GetCategoriasAsync(TipoCategoria.Herramienta);
            Categorias.Clear();
            foreach (var c in cats) Categorias.Add(c);

            var unds = await _catalogoService.GetUnidadesMedidaAsync();
            Unidades.Clear();
            foreach (var u in unds) Unidades.Add(u);
        }
        catch (Exception ex)
        {
            MostrarFeedback($"Error al cargar catálogos: {ex.Message}", false);
        }
    }

    [RelayCommand]
    public async Task CargarHerramientasAsync()
    {
        try
        {
            IsLoading = true;
            MensajeFeedback = null;

            int? catId = CategoriaFiltro?.CategoriaId;
            var list = await _herramientaService.BuscarAvanzadoAsync(Busqueda, catId, SoloBajoStock);

            Herramientas.Clear();
            decimal sumaStock = 0;
            int bajoStock = 0;

            foreach (var h in list)
            {
                Herramientas.Add(h);
                if (h.Stock != null)
                {
                    sumaStock += h.Stock.CantidadActual;
                    if ((h.Stock.CantidadActual - h.Stock.CantidadReservada) <= h.StockMinimo)
                    {
                        bajoStock++;
                    }
                }
            }

            TotalHerramientas = Herramientas.Count;
            TotalStockUnidades = sumaStock;
            AlertasStockBajo = bajoStock;
        }
        catch (Exception ex)
        {
            MostrarFeedback($"Error al consultar herramientas: {ex.Message}", false);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LimpiarFiltrosAsync()
    {
        Busqueda = string.Empty;
        CategoriaFiltro = null;
        SoloBajoStock = false;
        await CargarHerramientasAsync();
    }

    [RelayCommand]
    private async Task AbrirNuevoAsync()
    {
        IsEditMode = false;
        IsCreateMode = true;
        FormId = 0;
        FormCodigo = await _herramientaService.GenerarCodigoAsync();
        FormNombre = string.Empty;
        FormDescripcion = string.Empty;
        FormMarca = string.Empty;
        FormModelo = string.Empty;
        FormStockMinimo = 3;
        FormStockMaximo = 50;
        FormPrecioUnitario = 0;
        FormStockInicial = 1;
        FormActivo = true;

        if (Categorias.Any()) FormCategoriaId = Categorias.First().CategoriaId;
        if (Unidades.Any()) FormUnidadId = Unidades.First().UnidadId;

        IsFormOpen = true;
    }

    [RelayCommand]
    private void AbrirEditar(Herramienta? herramienta)
    {
        var target = herramienta ?? HerramientaSeleccionada;
        if (target == null) return;

        IsEditMode = true;
        IsCreateMode = false;
        FormId = target.HerramientaId;
        FormCodigo = target.Codigo;
        FormNombre = target.Nombre;
        FormDescripcion = target.Descripcion;
        FormMarca = target.Marca;
        FormModelo = target.Modelo;
        FormCategoriaId = target.CategoriaId;
        FormUnidadId = target.UnidadId;
        FormStockMinimo = target.StockMinimo;
        FormStockMaximo = target.StockMaximo;
        FormPrecioUnitario = target.PrecioUnitario;
        FormStockInicial = target.Stock?.CantidadActual ?? 0;
        FormActivo = target.Activo;

        IsFormOpen = true;
    }

    [RelayCommand]
    private void CancelarForm()
    {
        IsFormOpen = false;
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(FormNombre))
        {
            MostrarFeedback("El nombre de la herramienta es requerido.", false);
            return;
        }

        if (FormCategoriaId <= 0)
        {
            MostrarFeedback("Seleccione una categoría válida.", false);
            return;
        }

        if (FormUnidadId <= 0)
        {
            MostrarFeedback("Seleccione una unidad de medida.", false);
            return;
        }

        try
        {
            IsLoading = true;

            if (IsEditMode)
            {
                var h = new Herramienta
                {
                    HerramientaId = FormId,
                    Codigo = FormCodigo,
                    Nombre = FormNombre.Trim(),
                    Descripcion = FormDescripcion?.Trim(),
                    Marca = FormMarca?.Trim(),
                    Modelo = FormModelo?.Trim(),
                    CategoriaId = FormCategoriaId,
                    UnidadId = FormUnidadId,
                    StockMinimo = FormStockMinimo,
                    StockMaximo = FormStockMaximo,
                    PrecioUnitario = FormPrecioUnitario,
                    Activo = FormActivo
                };

                var (exito, mensaje) = await _herramientaService.ActualizarAsync(h);
                if (exito)
                {
                    MostrarFeedback("Herramienta actualizada con éxito.", true);
                    IsFormOpen = false;
                    await CargarHerramientasAsync();
                }
                else
                {
                    MostrarFeedback(mensaje, false);
                }
            }
            else
            {
                var h = new Herramienta
                {
                    Codigo = FormCodigo.Trim(),
                    Nombre = FormNombre.Trim(),
                    Descripcion = FormDescripcion?.Trim(),
                    Marca = FormMarca?.Trim(),
                    Modelo = FormModelo?.Trim(),
                    CategoriaId = FormCategoriaId,
                    UnidadId = FormUnidadId,
                    StockMinimo = FormStockMinimo,
                    StockMaximo = FormStockMaximo,
                    PrecioUnitario = FormPrecioUnitario,
                    Activo = FormActivo
                };

                var (exito, mensaje, nueva) = await _herramientaService.CrearConStockInicialAsync(h, FormStockInicial);
                if (exito)
                {
                    MostrarFeedback("Herramienta y stock inicial registrados con éxito.", true);
                    IsFormOpen = false;
                    await CargarHerramientasAsync();
                }
                else
                {
                    MostrarFeedback(mensaje, false);
                }
            }
        }
        catch (Exception ex)
        {
            MostrarFeedback($"Error al guardar: {ex.Message}", false);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CambiarEstadoAsync(Herramienta? herramienta)
    {
        var target = herramienta ?? HerramientaSeleccionada;
        if (target == null) return;

        var accion = target.Activo ? "desactivar" : "activar";
        var res = MessageBox.Show(
            $"¿Está seguro de que desea {accion} la herramienta '{target.Nombre}' ({target.Codigo})?",
            "Confirmación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (res == MessageBoxResult.Yes)
        {
            var (exito, mensaje) = await _herramientaService.ActivarDesactivarAsync(target.HerramientaId);
            MostrarFeedback(mensaje, exito);
            await CargarHerramientasAsync();
        }
    }

    private void MostrarFeedback(string mensaje, bool esExito)
    {
        MensajeFeedback = mensaje;
        IsFeedbackSuccess = esExito;
    }
}
