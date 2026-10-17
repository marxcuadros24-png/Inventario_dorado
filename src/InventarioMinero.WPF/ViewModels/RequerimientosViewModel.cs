using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.Models.Entities;
using InventarioMinero.Models.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace InventarioMinero.WPF.ViewModels;

public partial class RequerimientosViewModel : ObservableObject
{
    private readonly IRequerimientoService _requerimientoService;

    public ObservableCollection<Requerimiento> Requerimientos { get; } = new();

    // Filtros
    [ObservableProperty]
    private string _busqueda = string.Empty;

    [ObservableProperty]
    private bool _soloPendientes;

    // Estado UI
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private Requerimiento? _requerimientoSeleccionado;

    [ObservableProperty]
    private Requerimiento? _requerimientoParaImprimir;

    [ObservableProperty]
    private bool _isPrintPreviewOpen;

    [ObservableProperty]
    private string? _mensajeFeedback;

    [ObservableProperty]
    private bool _isFeedbackSuccess;

    [ObservableProperty]
    private bool _hayStockBajo;

    // KPIs
    [ObservableProperty]
    private int _totalRequerimientos;

    [ObservableProperty]
    private int _requerimientosPendientes;

    [ObservableProperty]
    private int _requerimientosAtendidos;

    // Evento para que la vista ejecute el comando de impresión física WPF (PrintDialog)
    public event EventHandler<FrameworkElement>? RequestPrintVisual;

    public RequerimientosViewModel(IRequerimientoService requerimientoService)
    {
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
            await CargarRequerimientosAsync();
            await VerificarStockBajoAsync();
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task VerificarStockBajoAsync()
    {
        try
        {
            HayStockBajo = await _requerimientoService.HayItemsConStockBajoAsync();
        }
        catch
        {
            HayStockBajo = false;
        }
    }

    [RelayCommand]
    public async Task CargarRequerimientosAsync()
    {
        try
        {
            IsLoading = true;
            MensajeFeedback = null;

            var list = SoloPendientes 
                ? await _requerimientoService.GetPendientesAsync()
                : await _requerimientoService.GetTodosAsync();

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                var b = Busqueda.Trim().ToLower();
                list = list.Where(r => 
                    r.Numero.ToLower().Contains(b) ||
                    (r.Usuario != null && r.Usuario.NombreUsuario.ToLower().Contains(b)) ||
                    (r.Area != null && r.Area.ToLower().Contains(b)));
            }

            Requerimientos.Clear();
            int pendientes = 0;
            int atendidos = 0;

            foreach (var r in list)
            {
                Requerimientos.Add(r);
                if (r.Estado == EstadoRequerimiento.Pendiente) pendientes++;
                else if (r.Estado == EstadoRequerimiento.Atendido) atendidos++;
            }

            TotalRequerimientos = Requerimientos.Count;
            RequerimientosPendientes = pendientes;
            RequerimientosAtendidos = atendidos;

            if (RequerimientoSeleccionado != null)
            {
                RequerimientoSeleccionado = Requerimientos.FirstOrDefault(r => r.RequerimientoId == RequerimientoSeleccionado.RequerimientoId);
            }
        }
        catch (Exception ex)
        {
            MostrarFeedback($"Error al cargar requerimientos: {ex.Message}", false);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GenerarAutomaticoAsync(int? usuarioId)
    {
        try
        {
            IsLoading = true;
            int id = usuarioId ?? 1;

            var (exito, mensaje, req) = await _requerimientoService.GenerarPorStockBajoAsync(id, "Almacén Central y Planta");

            if (exito && req != null)
            {
                MostrarFeedback(mensaje, true);
                await CargarRequerimientosAsync();
                await VerificarStockBajoAsync();

                // Abrir directamente la vista previa de impresión
                AbrirVistaPrevia(req);
            }
            else
            {
                MostrarFeedback(mensaje, false);
            }
        }
        catch (Exception ex)
        {
            MostrarFeedback($"Error al generar requerimiento: {ex.Message}", false);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task VerDetalleAsync(Requerimiento? req)
    {
        var target = req ?? RequerimientoSeleccionado;
        if (target == null) return;

        var fullReq = await _requerimientoService.GetPorIdAsync(target.RequerimientoId);
        AbrirVistaPrevia(fullReq ?? target);
    }

    [RelayCommand]
    private void AbrirVistaPrevia(Requerimiento req)
    {
        RequerimientoParaImprimir = req;
        IsPrintPreviewOpen = true;
    }

    [RelayCommand]
    private void CerrarVistaPrevia()
    {
        IsPrintPreviewOpen = false;
    }

    [RelayCommand]
    private async Task ImprimirHojaAsync(FrameworkElement visualToPrint)
    {
        if (RequerimientoParaImprimir == null || visualToPrint == null) return;

        try
        {
            // Disparar evento para que la vista ejecute el PrintDialog nativo de WPF
            RequestPrintVisual?.Invoke(this, visualToPrint);

            // Registrar fecha de impresión en base de datos
            await _requerimientoService.MarcarImpresionAsync(RequerimientoParaImprimir.RequerimientoId);
            RequerimientoParaImprimir.FechaImpresion = DateTime.Now;

            MostrarFeedback($"Requerimiento {RequerimientoParaImprimir.Numero} enviado a imprimir correctamente.", true);
            await CargarRequerimientosAsync();
        }
        catch (Exception ex)
        {
            MostrarFeedback($"Error al imprimir: {ex.Message}", false);
        }
    }

    [RelayCommand]
    private async Task AnularAsync(Requerimiento? req)
    {
        var target = req ?? RequerimientoSeleccionado;
        if (target == null) return;

        var res = MessageBox.Show(
            $"¿Está seguro de que desea anular el requerimiento {target.Numero}?",
            "Confirmación de Anulación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (res == MessageBoxResult.Yes)
        {
            var (exito, mensaje) = await _requerimientoService.AnularAsync(target.RequerimientoId);
            MostrarFeedback(mensaje, exito);
            await CargarRequerimientosAsync();
            await VerificarStockBajoAsync();
        }
    }

    private void MostrarFeedback(string mensaje, bool esExito)
    {
        MensajeFeedback = mensaje;
        IsFeedbackSuccess = esExito;
    }
}
