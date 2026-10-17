using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace InventarioMinero.WPF.ViewModels;

public class ItemMovimientoDraft : ObservableObject
{
    public Herramienta Herramienta { get; set; } = null!;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}

public partial class MovimientosViewModel : ObservableObject
{
    private readonly IMovimientoService _movimientoService;
    private readonly IHerramientaService _herramientaService;
    private readonly IRequerimientoService _requerimientoService;

    public ObservableCollection<Movimiento> Movimientos { get; } = new();
    public int UsuarioActualId { get; set; } = 1;
    public ObservableCollection<TipoMovimiento> TiposMovimiento { get; } = new();
    public ObservableCollection<Herramienta> CatalogoHerramientas { get; } = new();
    public ObservableCollection<Requerimiento> RequerimientosPendientes { get; } = new();

    // Filtros
    [ObservableProperty]
    private string _busqueda = string.Empty;

    [ObservableProperty]
    private TipoMovimiento? _tipoFiltro;

    [ObservableProperty]
    private DateTime? _fechaDesde;

    [ObservableProperty]
    private DateTime? _fechaHasta;

    // Estado UI
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private Movimiento? _movimientoSeleccionado;

    [ObservableProperty]
    private bool _isNuevoMovimientoOpen;

    [ObservableProperty]
    private bool _isDetalleModalOpen;

    [ObservableProperty]
    private bool _isKardexModalOpen;

    [ObservableProperty]
    private string? _mensajeFeedback;

    [ObservableProperty]
    private bool _isFeedbackSuccess;

    // KPIs
    [ObservableProperty]
    private int _totalMovimientos;

    [ObservableProperty]
    private int _totalEntradas;

    [ObservableProperty]
    private int _totalSalidas;

    // ── CAMPOS DEL FORMULARIO DE NUEVO MOVIMIENTO ──
    [ObservableProperty]
    private int _formTipoMovId;

    [ObservableProperty]
    private string? _formDocumentoRef;

    [ObservableProperty]
    private string? _formObservaciones;

    [ObservableProperty]
    private int? _formRequerimientoId;

    // Ítem temporal a agregar al borrador
    [ObservableProperty]
    private Herramienta? _itemHerramientaSeleccionada;

    [ObservableProperty]
    private decimal _itemCantidad = 1;

    [ObservableProperty]
    private decimal _itemPrecioUnitario = 0;

    public ObservableCollection<ItemMovimientoDraft> ItemsBorrador { get; } = new();

    [ObservableProperty]
    private decimal _totalBorrador;

    // Kárdex por Herramienta
    [ObservableProperty]
    private Herramienta? _herramientaKardex;

    public ObservableCollection<DetalleMovimiento> KardexDetalles { get; } = new();

    public MovimientosViewModel(
        IMovimientoService movimientoService,
        IHerramientaService herramientaService,
        IRequerimientoService requerimientoService)
    {
        _movimientoService = movimientoService;
        _herramientaService = herramientaService;
        _requerimientoService = requerimientoService;
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
            await CargarMovimientosAsync();
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
            var tipos = await _movimientoService.GetTiposMovimientoAsync();
            TiposMovimiento.Clear();
            foreach (var t in tipos) TiposMovimiento.Add(t);

            var htms = await _herramientaService.GetTodasAsync(soloActivas: true);
            CatalogoHerramientas.Clear();
            foreach (var h in htms) CatalogoHerramientas.Add(h);

            var reqs = await _requerimientoService.GetPendientesAsync();
            RequerimientosPendientes.Clear();
            foreach (var r in reqs) RequerimientosPendientes.Add(r);
        }
        catch (Exception ex)
        {
            MostrarFeedback($"Error al cargar catálogos: {ex.Message}", false);
        }
    }

    [RelayCommand]
    public async Task CargarMovimientosAsync()
    {
        try
        {
            IsLoading = true;
            MensajeFeedback = null;

            int? tipoId = TipoFiltro?.TipoMovId;
            var list = await _movimientoService.BuscarAvanzadoAsync(Busqueda, tipoId, FechaDesde, FechaHasta);

            Movimientos.Clear();
            int entradas = 0;
            int salidas = 0;

            foreach (var m in list)
            {
                Movimientos.Add(m);
                if (m.Estado == EstadoMovimiento.Confirmado)
                {
                    if (m.TipoMovimiento?.EsEntrada == true) entradas++;
                    else salidas++;
                }
            }

            TotalMovimientos = Movimientos.Count;
            TotalEntradas = entradas;
            TotalSalidas = salidas;
        }
        catch (Exception ex)
        {
            MostrarFeedback($"Error al consultar movimientos: {ex.Message}", false);
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
        TipoFiltro = null;
        FechaDesde = null;
        FechaHasta = null;
        await CargarMovimientosAsync();
    }

    [RelayCommand]
    private async Task AbrirNuevoAsync()
    {
        await CargarCatalogosAsync();

        FormTipoMovId = TiposMovimiento.FirstOrDefault()?.TipoMovId ?? 1;
        FormDocumentoRef = string.Empty;
        FormObservaciones = string.Empty;
        FormRequerimientoId = null;

        ItemsBorrador.Clear();
        TotalBorrador = 0;
        ItemHerramientaSeleccionada = CatalogoHerramientas.FirstOrDefault();
        ItemCantidad = 1;
        ItemPrecioUnitario = ItemHerramientaSeleccionada?.PrecioUnitario ?? 0;

        IsNuevoMovimientoOpen = true;
    }

    partial void OnItemHerramientaSeleccionadaChanged(Herramienta? value)
    {
        if (value != null)
        {
            ItemPrecioUnitario = value.PrecioUnitario;
        }
    }

    [RelayCommand]
    private void AgregarItemBorrador()
    {
        if (ItemHerramientaSeleccionada == null)
        {
            MostrarFeedback("Seleccione una herramienta para agregar.", false);
            return;
        }

        if (ItemCantidad <= 0)
        {
            MostrarFeedback("La cantidad debe ser mayor a cero.", false);
            return;
        }

        // Si ya está en la lista, sumar cantidad
        var existente = ItemsBorrador.FirstOrDefault(i => i.Herramienta.HerramientaId == ItemHerramientaSeleccionada.HerramientaId);
        if (existente != null)
        {
            existente.Cantidad += ItemCantidad;
            existente.PrecioUnitario = ItemPrecioUnitario;
        }
        else
        {
            ItemsBorrador.Add(new ItemMovimientoDraft
            {
                Herramienta = ItemHerramientaSeleccionada,
                Cantidad = ItemCantidad,
                PrecioUnitario = ItemPrecioUnitario
            });
        }

        CalcularTotalBorrador();
        ItemCantidad = 1;
    }

    [RelayCommand]
    private void EliminarItemBorrador(ItemMovimientoDraft item)
    {
        if (item != null)
        {
            ItemsBorrador.Remove(item);
            CalcularTotalBorrador();
        }
    }

    private void CalcularTotalBorrador()
    {
        TotalBorrador = ItemsBorrador.Sum(i => i.Subtotal);
    }

    [RelayCommand]
    private void CancelarNuevo()
    {
        IsNuevoMovimientoOpen = false;
    }

    [RelayCommand]
    private async Task GuardarMovimientoAsync(int? usuarioId)
    {
        if (!ItemsBorrador.Any())
        {
            MostrarFeedback("Agregue al menos una herramienta al movimiento.", false);
            return;
        }

        int userId = usuarioId ?? 1;

        var items = ItemsBorrador.Select(i => (
            i.Herramienta.HerramientaId,
            i.Cantidad,
            i.PrecioUnitario
        )).ToList();

        try
        {
            IsLoading = true;
            var (exito, mensaje, numero) = await _movimientoService.RegistrarAsync(
                FormTipoMovId,
                userId,
                items,
                FormDocumentoRef,
                FormObservaciones,
                FormRequerimientoId);

            if (exito)
            {
                MostrarFeedback(mensaje, true);
                IsNuevoMovimientoOpen = false;
                await CargarMovimientosAsync();
            }
            else
            {
                MostrarFeedback(mensaje, false);
            }
        }
        catch (Exception ex)
        {
            MostrarFeedback($"Error al registrar movimiento: {ex.Message}", false);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task VerDetalleAsync(Movimiento? mov)
    {
        var target = mov ?? MovimientoSeleccionado;
        if (target == null) return;

        var fullMov = await _movimientoService.GetPorIdAsync(target.MovimientoId);
        MovimientoSeleccionado = fullMov ?? target;
        IsDetalleModalOpen = true;
    }

    [RelayCommand]
    private void CerrarDetalle()
    {
        IsDetalleModalOpen = false;
    }

    [RelayCommand]
    private async Task AnularAsync(Movimiento? mov)
    {
        var target = mov ?? MovimientoSeleccionado;
        if (target == null) return;

        var res = MessageBox.Show(
            $"¿Está seguro de que desea anular el movimiento {target.Numero}?\n\nEsta acción revertirá automáticamente las cantidades en el stock físico de almacén.",
            "Confirmación de Anulación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (res == MessageBoxResult.Yes)
        {
            var (exito, mensaje) = await _movimientoService.AnularAsync(target.MovimientoId, 1);
            MostrarFeedback(mensaje, exito);
            await CargarMovimientosAsync();
            if (IsDetalleModalOpen) IsDetalleModalOpen = false;
        }
    }

    [RelayCommand]
    private async Task VerKardexAsync(Herramienta? htm)
    {
        if (htm == null) return;

        HerramientaKardex = htm;
        var detalles = await _movimientoService.GetKardexPorHerramientaAsync(htm.HerramientaId);

        KardexDetalles.Clear();
        foreach (var d in detalles) KardexDetalles.Add(d);

        IsKardexModalOpen = true;
    }

    [RelayCommand]
    private void CerrarKardex()
    {
        IsKardexModalOpen = false;
    }

    private void MostrarFeedback(string mensaje, bool esExito)
    {
        MensajeFeedback = mensaje;
        IsFeedbackSuccess = esExito;
    }
}
