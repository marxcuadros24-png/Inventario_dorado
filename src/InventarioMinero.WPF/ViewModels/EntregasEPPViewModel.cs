using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace InventarioMinero.WPF.ViewModels;

public class DetalleEPPDraftItem : ObservableObject
{
    public EPP EPP { get; set; } = null!;
    public decimal Cantidad { get; set; } = 1;
    public decimal PrecioUnitario { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}

public partial class EntregasEPPViewModel : ObservableObject
{
    private readonly IEntregaEPPService _entregaService;
    private readonly IEPPService _eppService;
    private readonly IEmpleadoService _empleadoService;

    [ObservableProperty]
    private ObservableCollection<EntregaEPP> _entregas = new();

    [ObservableProperty]
    private ObservableCollection<EPP> _catalogoEPP = new();

    [ObservableProperty]
    private ObservableCollection<Empleado> _empleados = new();

    [ObservableProperty]
    private ObservableCollection<string> _motivos = new() { "Dotacion", "Reposicion", "Deterioro", "Perdida" };

    [ObservableProperty]
    private ObservableCollection<DetalleEPPDraftItem> _itemsBorrador = new();

    [ObservableProperty]
    private EntregaEPP? _entregaSeleccionada;

    // Filtros
    [ObservableProperty]
    private string _busqueda = string.Empty;

    [ObservableProperty]
    private DateTime? _fechaDesde;

    [ObservableProperty]
    private DateTime? _fechaHasta;

    // KPIs
    [ObservableProperty]
    private int _kpiTotalEntregas;

    [ObservableProperty]
    private decimal _kpiTotalUnidadesEntregadas;

    [ObservableProperty]
    private int _kpiTrabajadoresDotados;

    // Modales
    [ObservableProperty]
    private bool _mostrarModalNueva;

    [ObservableProperty]
    private bool _mostrarModalImpresion;

    // Formulario Nueva Entrega
    [ObservableProperty]
    private Empleado? _nuevoEmpleado;

    [ObservableProperty]
    private string _nuevoMotivo = "Dotacion";

    [ObservableProperty]
    private string? _nuevasObservaciones;

    // Selector de item temporal
    [ObservableProperty]
    private EPP? _itemEPPSeleccionado;

    [ObservableProperty]
    private decimal _itemCantidad = 1;

    [ObservableProperty]
    private decimal _stockDisponibleItem;

    // Alertas y estado
    [ObservableProperty]
    private string? _mensajeBanner;

    [ObservableProperty]
    private bool _esErrorBanner;

    [ObservableProperty]
    private bool _estaCargando;

    public int UsuarioActualId { get; set; } = 1;

    public event EventHandler<FrameworkElement>? RequestPrintVisual;

    public EntregasEPPViewModel(
        IEntregaEPPService entregaService,
        IEPPService eppService,
        IEmpleadoService empleadoService)
    {
        _entregaService = entregaService;
        _eppService = eppService;
        _empleadoService = empleadoService;
    }

    private readonly SemaphoreSlim _initLock = new(1, 1);

    public async Task InicializarAsync()
    {
        if (!await _initLock.WaitAsync(0)) return;
        try
        {
            MensajeBanner = null;
            await CargarCatalogosAsync();
            await CargarEntregasAsync();
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
            var eList = await _empleadoService.GetTodosAsync(soloActivos: true);
            Empleados = new ObservableCollection<Empleado>(eList);

            var epps = await _eppService.GetTodosAsync(soloActivos: true);
            CatalogoEPP = new ObservableCollection<EPP>(epps);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al cargar datos base: {ex.Message}", true);
        }
    }

    [RelayCommand]
    public async Task CargarEntregasAsync()
    {
        EstaCargando = true;
        try
        {
            var lista = await _entregaService.GetTodasAsync(FechaDesde, FechaHasta);
            var todas = lista.ToList();

            var filtradas = todas.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                var term = Busqueda.Trim().ToLower();
                filtradas = filtradas.Where(e =>
                    e.Numero.ToLower().Contains(term) ||
                    e.Empleado.NombreCompleto.ToLower().Contains(term) ||
                    e.Empleado.DNI.Contains(term) ||
                    e.Detalles.Any(d => d.EPP.Nombre.ToLower().Contains(term)));
            }

            Entregas = new ObservableCollection<EntregaEPP>(filtradas);

            // Calcular KPIs
            KpiTotalEntregas = todas.Count;
            KpiTotalUnidadesEntregadas = todas.SelectMany(e => e.Detalles).Sum(d => d.Cantidad);
            KpiTrabajadoresDotados = todas.Select(e => e.EmpleadoId).Distinct().Count();
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al cargar entregas de EPP: {ex.Message}", true);
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public void AbrirModalNueva()
    {
        NuevoEmpleado = Empleados.FirstOrDefault();
        NuevoMotivo = "Dotacion";
        NuevasObservaciones = string.Empty;
        ItemsBorrador.Clear();

        ItemEPPSeleccionado = CatalogoEPP.FirstOrDefault();
        ItemCantidad = 1;
        ActualizarStockEPP();

        MostrarModalNueva = true;
    }

    [RelayCommand]
    public void CerrarModalNueva()
    {
        MostrarModalNueva = false;
    }

    partial void OnItemEPPSeleccionadoChanged(EPP? value)
    {
        ActualizarStockEPP();
    }

    private void ActualizarStockEPP()
    {
        StockDisponibleItem = ItemEPPSeleccionado?.StockActual ?? 0;
    }

    [RelayCommand]
    public void AgregarItemBorrador()
    {
        if (ItemEPPSeleccionado == null)
        {
            MostrarMensaje("Seleccione un EPP para agregar.", true);
            return;
        }

        if (ItemCantidad <= 0)
        {
            MostrarMensaje("La cantidad debe ser mayor a 0.", true);
            return;
        }

        if (ItemCantidad > StockDisponibleItem)
        {
            MostrarMensaje($"Stock insuficiente. Solo hay {StockDisponibleItem:N0} unidades disponibles en almacén.", true);
            return;
        }

        if (ItemsBorrador.Any(i => i.EPP.EPPId == ItemEPPSeleccionado.EPPId))
        {
            MostrarMensaje("Este EPP ya está agregado en la lista. Modifique su cantidad o elimínelo.", true);
            return;
        }

        var mesesVida = ItemEPPSeleccionado.VidaUtilMeses.GetValueOrDefault(6);
        ItemsBorrador.Add(new DetalleEPPDraftItem
        {
            EPP = ItemEPPSeleccionado,
            Cantidad = ItemCantidad,
            PrecioUnitario = ItemEPPSeleccionado.PrecioUnitario,
            FechaVencimiento = DateTime.Today.AddMonths(mesesVida)
        });

        ItemCantidad = 1;
    }

    [RelayCommand]
    public void EliminarItemBorrador(DetalleEPPDraftItem? item)
    {
        if (item != null)
        {
            ItemsBorrador.Remove(item);
        }
    }

    [RelayCommand]
    public async Task GuardarEntregaAsync()
    {
        if (NuevoEmpleado == null)
        {
            MostrarMensaje("Debe seleccionar al trabajador minero.", true);
            return;
        }

        if (ItemsBorrador.Count == 0)
        {
            MostrarMensaje("Debe agregar al menos un EPP a la lista de dotación.", true);
            return;
        }

        EstaCargando = true;
        try
        {
            var itemsParam = ItemsBorrador.Select(i => (
                EPPId: i.EPP.EPPId,
                Cantidad: i.Cantidad,
                Precio: i.PrecioUnitario,
                FechaVencimiento: (DateTime?)i.FechaVencimiento
            )).ToList();

            var res = await _entregaService.RegistrarAsync(
                NuevoEmpleado.EmpleadoId,
                UsuarioActualId,
                NuevoMotivo,
                itemsParam,
                NuevasObservaciones);

            if (res.Exito)
            {
                MostrarModalNueva = false;
                MostrarMensaje(res.Mensaje, false);
                await CargarCatalogosAsync();
                await CargarEntregasAsync();
            }
            else
            {
                MostrarMensaje(res.Mensaje, true);
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al procesar dotación: {ex.Message}", true);
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public void VerCargoEntrega(EntregaEPP? entrega)
    {
        var target = entrega ?? EntregaSeleccionada;
        if (target == null)
        {
            MostrarMensaje("Seleccione un registro de entrega para ver o imprimir su cargo.", true);
            return;
        }

        EntregaSeleccionada = target;
        MostrarModalImpresion = true;
    }

    [RelayCommand]
    public void CerrarModalImpresion()
    {
        MostrarModalImpresion = false;
    }

    [RelayCommand]
    public void ImprimirCargo(FrameworkElement? visual)
    {
        if (visual != null)
        {
            RequestPrintVisual?.Invoke(this, visual);
        }
    }

    private void MostrarMensaje(string mensaje, bool esError)
    {
        MensajeBanner = mensaje;
        EsErrorBanner = esError;

        _ = Task.Run(async () =>
        {
            await Task.Delay(5000);
            if (MensajeBanner == mensaje)
            {
                MensajeBanner = null;
            }
        });
    }
}
