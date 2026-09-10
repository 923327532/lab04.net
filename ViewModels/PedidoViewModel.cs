using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels;

public class PedidoViewModel : ViewModelBase
{
    private readonly PedidoRepositorio _repo;
    private readonly CatalogoRepositorio _catalogo;

    private Pedido? _pedidoSeleccionado;
    private DetalleLinea? _detalleSeleccionada;
    private bool _esNuevo;
    private int _cantidadLinea = 1;
    private decimal _descuentoLinea;
    private DateTime? _fechaInicio;
    private DateTime? _fechaFin;

    public ObservableCollection<Pedido> Pedidos { get; } = new();
    public ObservableCollection<DetalleLinea> DetalleActual { get; } = new();
    public ObservableCollection<DetalleLinea> ReporteDetalles { get; } = new();

    public ObservableCollection<Cliente> Clientes { get; } = new();
    public ObservableCollection<Empleado> Empleados { get; } = new();
    public ObservableCollection<Transportista> Transportistas { get; } = new();
    public ObservableCollection<Producto> Productos { get; } = new();

    private Producto? _productoSeleccionado;
    public Producto? ProductoSeleccionado
    {
        get => _productoSeleccionado;
        set => SetProperty(ref _productoSeleccionado, value);
    }

    public ICommand CargarCommand { get; }
    public ICommand NuevoCommand { get; }
    public ICommand GuardarCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand CancelarCommand { get; }
    public ICommand AgregarLineaCommand { get; }
    public ICommand QuitarLineaCommand { get; }
    public ICommand GenerarReporteCommand { get; }

    public PedidoViewModel()
    {
        _repo = new PedidoRepositorio();
        _catalogo = new CatalogoRepositorio();

        CargarCommand = new RelayCommand(_ => Cargar());
        NuevoCommand = new RelayCommand(_ => Nuevo(), _ => !_esNuevo);
        GuardarCommand = new RelayCommand(_ => Guardar());
        EliminarCommand = new RelayCommand(_ => Eliminar(), _ => !_esNuevo);
        CancelarCommand = new RelayCommand(_ => Cancelar(), _ => _esNuevo);
        AgregarLineaCommand = new RelayCommand(_ => AgregarLinea(), _ => PuedeGestionarLineas);
        QuitarLineaCommand = new RelayCommand(_ => QuitarLinea(), _ => PuedeGestionarLineas);
        GenerarReporteCommand = new RelayCommand(_ => GenerarReporte());
    }

    public bool PuedeGestionarLineas => !_esNuevo && _pedidoSeleccionado is { PedidoID: > 0 };

    public Pedido? PedidoSeleccionado
    {
        get => _pedidoSeleccionado;
        set
        {
            if (SetProperty(ref _pedidoSeleccionado, value)) CargarLineasDeSeleccionado();
        }
    }

    public DetalleLinea? DetalleSeleccionada
    {
        get => _detalleSeleccionada;
        set => SetProperty(ref _detalleSeleccionada, value);
    }

    public int CantidadLinea
    {
        get => _cantidadLinea;
        set => SetProperty(ref _cantidadLinea, value);
    }

    public decimal DescuentoLinea
    {
        get => _descuentoLinea;
        set => SetProperty(ref _descuentoLinea, value);
    }

    public DateTime? FechaInicio
    {
        get => _fechaInicio;
        set => SetProperty(ref _fechaInicio, value);
    }

    public DateTime? FechaFin
    {
        get => _fechaFin;
        set => SetProperty(ref _fechaFin, value);
    }

    public void Cargar()
    {
        CargarCombos();
        RecargarPedidos();
        ReporteDetalles.Clear();
    }

    private void CargarCombos()
    {
        Clientes.Clear();
        foreach (var c in _catalogo.Clientes()) Clientes.Add(c);

        Empleados.Clear();
        foreach (var e in _catalogo.Empleados()) Empleados.Add(e);

        Transportistas.Clear();
        foreach (var t in _catalogo.Transportistas()) Transportistas.Add(t);

        Productos.Clear();
        foreach (var p in _catalogo.Productos()) Productos.Add(p);
    }

    private void RecargarPedidos()
    {
        Pedidos.Clear();
        foreach (var p in _repo.ListarPedidos()) Pedidos.Add(p);
        OnPropertyChanged(nameof(Pedidos));
    }

    private void Nuevo()
    {
        _esNuevo = true;
        var nuevo = new Pedido
        {
            PedidoID = 0,
            FechaPedido = DateTime.Today,
            FechaRequerida = DateTime.Today.AddDays(7)
        };
        Pedidos.Insert(0, nuevo);
        DetalleActual.Clear();
        PedidoSeleccionado = nuevo;
        Notificar();
    }

    private void Guardar()
    {
        if (PedidoSeleccionado == null || PedidoSeleccionado.ClienteID == null)
        {
            MessageBox.Show("Seleccione un pedido y asigne un cliente.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var pedido = PedidoSeleccionado;
        if (_esNuevo)
        {
            pedido.PedidoID = _repo.InsertarPedido(pedido);
            _esNuevo = false;
        }
        else
        {
            _repo.ActualizarPedido(pedido);
        }
        RecargarPedidos();
        PedidoSeleccionado = Pedidos.FirstOrDefault(x => x.PedidoID == pedido.PedidoID);
        CargarDetalleActual(pedido.PedidoID);
        Notificar();
    }

    private void Eliminar()
    {
        if (PedidoSeleccionado == null) return;
        var pedido = PedidoSeleccionado;
        if (!Confirmar($"Desea eliminar el pedido {pedido.PedidoID} y sus detalles?")) return;
        _repo.EliminarPedido(pedido.PedidoID);
        Pedidos.Remove(pedido);
        DetalleActual.Clear();
        PedidoSeleccionado = null;
    }

    private void Cancelar()
    {
        if (_esNuevo)
        {
            var temp = PedidoSeleccionado;
            if (temp != null) Pedidos.Remove(temp);
        }
        _esNuevo = false;
        PedidoSeleccionado = null;
        Notificar();
    }


    private void CargarLineasDeSeleccionado()
    {
        OnPropertyChanged(nameof(PuedeGestionarLineas));
        if (PedidoSeleccionado is { PedidoID: > 0 })
            CargarDetalleActual(PedidoSeleccionado.PedidoID);
        else
            DetalleActual.Clear();
    }

    private void CargarDetalleActual(int pedidoID)
    {
        DetalleActual.Clear();
        foreach (var d in _repo.ListarDetalle(pedidoID)) DetalleActual.Add(d);
    }

    private void AgregarLinea()
    {
        if (!PuedeGestionarLineas) return;
        var pedido = PedidoSeleccionado;
        if (ProductoSeleccionado == null)
        {
            MessageBox.Show("Seleccione un producto para la linea.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var linea = new DetalleLinea
        {
            PedidoID = pedido!.PedidoID,
            ProductoID = ProductoSeleccionado.ProductoID,
            NombreProducto = ProductoSeleccionado.NombreProducto,
            PrecioUnidad = ProductoSeleccionado.PrecioUnidad,
            Cantidad = (short)Math.Max(1, CantidadLinea),
            Descuento = Math.Clamp(DescuentoLinea, 0m, 1m)
        };
        _repo.AgregarDetalle(linea);
        CargarDetalleActual(pedido.PedidoID);
        DetalleSeleccionada = DetalleActual.Count > 0 ? DetalleActual[^1] : null;
    }

    private void QuitarLinea()
    {
        if (!PuedeGestionarLineas || DetalleSeleccionada == null || PedidoSeleccionado == null) return;
        if (!Confirmar("Desea quitar la linea seleccionada del pedido?")) return;
        _repo.QuitarDetalle(DetalleSeleccionada.PedidoID, DetalleSeleccionada.ProductoID);
        CargarDetalleActual(PedidoSeleccionado.PedidoID);
        DetalleSeleccionada = null;
    }

    private void GenerarReporte()
    {
        if (FechaInicio != null && FechaFin != null && FechaInicio > FechaFin)
        {
            MessageBox.Show("La fecha de inicio no puede ser mayor que la fecha de fin.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        ReporteDetalles.Clear();
        foreach (var d in _repo.ListarReporteFechas(FechaInicio, FechaFin)) ReporteDetalles.Add(d);
    }

    private void Notificar()
    {
        OnPropertyChanged(nameof(PedidoSeleccionado));
        OnPropertyChanged(nameof(PuedeGestionarLineas));
        CommandManager.InvalidateRequerySuggested();
    }

    private static bool Confirmar(string texto)
        => MessageBox.Show(texto, "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
}

