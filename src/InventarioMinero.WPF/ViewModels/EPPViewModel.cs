using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace InventarioMinero.WPF.ViewModels;

public partial class EPPViewModel : ObservableObject
{
    private readonly IEPPService _eppService;
    private readonly ICatalogoService _catalogoService;

    public ObservableCollection<EPP> EPPList { get; } = new();
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
    private EPP? _eppSeleccionado;

    [ObservableProperty]
    private bool _isFormOpen;

    [ObservableProperty]
    private bool _isEditMode;

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
    private string? _formTalla;

    [ObservableProperty]
    private int? _formVidaUtilMeses = 6;

    [ObservableProperty]
    private int _formCategoriaId;

    [ObservableProperty]
    private int _formUnidadId;

    [ObservableProperty]
    private decimal _formStockMinimo = 10;

    [ObservableProperty]
    private decimal? _formStockMaximo;

    [ObservableProperty]
    private decimal _formStockActual = 0;

    [ObservableProperty]
    private decimal _formPrecioUnitario = 0;

    [ObservableProperty]
    private bool _formActivo = true;

    // KPIs en tiempo real
    [ObservableProperty]
    private int _totalEPPs;

    [ObservableProperty]
    private decimal _totalStockUnidades;

    [ObservableProperty]
    private int _alertasStockBajo;

    public EPPViewModel(IEPPService eppService, ICatalogoService catalogoService)
    {
        _eppService = eppService;
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
            await CargarEPPAsync();
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
            var cats = await _catalogoService.GetCategoriasAsync(TipoCategoria.EPP);
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
    public async Task CargarEPPAsync()
    {
        try
        {
            IsLoading = true;
            MensajeFeedback = null;

            int? catId = CategoriaFiltro?.CategoriaId;
            var list = await _eppService.BuscarAvanzadoAsync(Busqueda, catId, SoloBajoStock);

            EPPList.Clear();
            decimal sumaStock = 0;
            int bajoStock = 0;

            foreach (var e in list)
            {
                EPPList.Add(e);
                sumaStock += e.StockActual;
                if (e.StockActual <= e.StockMinimo)
                {
                    bajoStock++;
                }
            }

            TotalEPPs = EPPList.Count;
            TotalStockUnidades = sumaStock;
            AlertasStockBajo = bajoStock;
        }
        catch (Exception ex)
        {
            MostrarFeedback($"Error al consultar EPP: {ex.Message}", false);
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
        await CargarEPPAsync();
    }

    [RelayCommand]
    private async Task AbrirNuevoAsync()
    {
        IsEditMode = false;
        FormId = 0;
        FormCodigo = await _eppService.GenerarCodigoAsync();
        FormNombre = string.Empty;
        FormDescripcion = string.Empty;
        FormMarca = string.Empty;
        FormTalla = string.Empty;
        FormVidaUtilMeses = 6;
        FormStockMinimo = 10;
        FormStockMaximo = 100;
        FormStockActual = 20;
        FormPrecioUnitario = 0;
        FormActivo = true;

        if (Categorias.Any()) FormCategoriaId = Categorias.First().CategoriaId;
        if (Unidades.Any()) FormUnidadId = Unidades.First().UnidadId;

        IsFormOpen = true;
    }

    [RelayCommand]
    private void AbrirEditar(EPP? epp)
    {
        var target = epp ?? EppSeleccionado;
        if (target == null) return;

        IsEditMode = true;
        FormId = target.EPPId;
        FormCodigo = target.Codigo;
        FormNombre = target.Nombre;
        FormDescripcion = target.Descripcion;
        FormMarca = target.Marca;
        FormTalla = target.Talla;
        FormVidaUtilMeses = target.VidaUtilMeses;
        FormCategoriaId = target.CategoriaId;
        FormUnidadId = target.UnidadId;
        FormStockMinimo = target.StockMinimo;
        FormStockMaximo = target.StockMaximo;
        FormStockActual = target.StockActual;
        FormPrecioUnitario = target.PrecioUnitario;
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
            MostrarFeedback("El nombre del equipo EPP es requerido.", false);
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
                var e = new EPP
                {
                    EPPId = FormId,
                    Codigo = FormCodigo,
                    Nombre = FormNombre.Trim(),
                    Descripcion = FormDescripcion?.Trim(),
                    Marca = FormMarca?.Trim(),
                    Talla = FormTalla?.Trim(),
                    VidaUtilMeses = FormVidaUtilMeses,
                    CategoriaId = FormCategoriaId,
                    UnidadId = FormUnidadId,
                    StockMinimo = FormStockMinimo,
                    StockMaximo = FormStockMaximo,
                    StockActual = FormStockActual,
                    PrecioUnitario = FormPrecioUnitario,
                    Activo = FormActivo
                };

                var (exito, mensaje) = await _eppService.ActualizarAsync(e);
                if (exito)
                {
                    MostrarFeedback("Equipo EPP actualizado con éxito.", true);
                    IsFormOpen = false;
                    await CargarEPPAsync();
                }
                else
                {
                    MostrarFeedback(mensaje, false);
                }
            }
            else
            {
                var e = new EPP
                {
                    Codigo = FormCodigo.Trim(),
                    Nombre = FormNombre.Trim(),
                    Descripcion = FormDescripcion?.Trim(),
                    Marca = FormMarca?.Trim(),
                    Talla = FormTalla?.Trim(),
                    VidaUtilMeses = FormVidaUtilMeses,
                    CategoriaId = FormCategoriaId,
                    UnidadId = FormUnidadId,
                    StockMinimo = FormStockMinimo,
                    StockMaximo = FormStockMaximo,
                    StockActual = FormStockActual,
                    PrecioUnitario = FormPrecioUnitario,
                    Activo = FormActivo
                };

                var (exito, mensaje, nuevo) = await _eppService.CrearAsync(e);
                if (exito)
                {
                    MostrarFeedback("Equipo EPP registrado exitosamente en almacén.", true);
                    IsFormOpen = false;
                    await CargarEPPAsync();
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
    private async Task CambiarEstadoAsync(EPP? epp)
    {
        var target = epp ?? EppSeleccionado;
        if (target == null) return;

        var accion = target.Activo ? "desactivar" : "activar";
        var res = MessageBox.Show(
            $"¿Está seguro de que desea {accion} el equipo '{target.Nombre}' ({target.Codigo})?",
            "Confirmación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (res == MessageBoxResult.Yes)
        {
            var (exito, mensaje) = await _eppService.ActivarDesactivarAsync(target.EPPId);
            MostrarFeedback(mensaje, exito);
            await CargarEPPAsync();
        }
    }

    private void MostrarFeedback(string mensaje, bool esExito)
    {
        MensajeFeedback = mensaje;
        IsFeedbackSuccess = esExito;
    }
}
