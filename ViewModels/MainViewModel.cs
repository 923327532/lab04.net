using System.Windows.Input;
using NeptunoApp.ViewModels;

namespace NeptunoApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly CategoriaViewModel _categorias = new();
    private readonly ProveedorViewModel _proveedores = new();
    private readonly ProductoViewModel _productos = new();
    private readonly PedidoViewModel _pedidos = new();

    private object? _contenido;
    private string _titulo = "Gestion de Neptuno";

    public ICommand MostrarCategoriasCommand { get; }
    public ICommand MostrarProveedoresCommand { get; }
    public ICommand MostrarProductosCommand { get; }
    public ICommand MostrarPedidosCommand { get; }

    public MainViewModel()
    {
        MostrarCategoriasCommand = new RelayCommand(_ => MostrarCategorias());
        MostrarProveedoresCommand = new RelayCommand(_ => MostrarProveedores());
        MostrarProductosCommand = new RelayCommand(_ => MostrarProductos());
        MostrarPedidosCommand = new RelayCommand(_ => MostrarPedidos());
        MostrarProductos();
    }

    public object? Contenido
    {
        get => _contenido;
        private set => SetProperty(ref _contenido, value);
    }

    public string Titulo
    {
        get => _titulo;
        private set => SetProperty(ref _titulo, value);
    }

    private void MostrarCategorias()
    {
        _categorias.Cargar();
        Contenido = _categorias;
        Titulo = "Mantenimiento de Categorias";
    }

    private void MostrarProveedores()
    {
        _proveedores.Cargar();
        Contenido = _proveedores;
        Titulo = "Mantenimiento de Proveedores y Busqueda";
    }

    private void MostrarProductos()
    {
        _productos.Cargar();
        Contenido = _productos;
        Titulo = "Mantenimiento de Productos";
    }

    private void MostrarPedidos()
    {
        _pedidos.Cargar();
        Contenido = _pedidos;
        Titulo = "Mantenimiento de Pedidos y Reporte de Detalles";
    }
}
