using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels;

public class ProductoViewModel : ViewModelBase
{
    private readonly ProductoRepositorio _repo;
    private readonly CatalogoRepositorio _catalogo;

    private Producto? _seleccionado;
    private bool _esNuevo;

    public ObservableCollection<Producto> Productos { get; } = new();
    public ObservableCollection<Categoria> Categorias { get; } = new();
    public ObservableCollection<Proveedor> Proveedores { get; } = new();

    public ICommand CargarCommand { get; }
    public ICommand NuevoCommand { get; }
    public ICommand GuardarCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand CancelarCommand { get; }

    public ProductoViewModel()
    {
        _repo = new ProductoRepositorio();
        _catalogo = new CatalogoRepositorio();

        CargarCommand = new RelayCommand(_ => Cargar());
        NuevoCommand = new RelayCommand(_ => Nuevo(), _ => !_esNuevo);
        GuardarCommand = new RelayCommand(_ => Guardar());
        EliminarCommand = new RelayCommand(_ => Eliminar(), _ => !_esNuevo);
        CancelarCommand = new RelayCommand(_ => Cancelar(), _ => _esNuevo);
    }

    public Producto? Seleccionado
    {
        get => _seleccionado;
        set => SetProperty(ref _seleccionado, value);
    }

    public void Cargar()
    {
        RecargarCombos();
        Productos.Clear();
        foreach (var p in _repo.Listar()) Productos.Add(p);
    }

    private void RecargarCombos()
    {
        Categorias.Clear();
        foreach (var c in _catalogo.Categorias()) Categorias.Add(c);

        Proveedores.Clear();
        foreach (var p in _catalogo.Proveedores()) Proveedores.Add(p);
    }

    private void Nuevo()
    {
        var nuevo = new Producto
        {
            ProductoID = 0,
            NombreProducto = "Nuevo producto",
            PrecioUnidad = 0,
            UnidadesEnExistencia = 0,
            UnidadesEnPedido = 0,
            NivelDeReorden = 0,
            Descontinuado = false
        };
        _esNuevo = true;
        Productos.Insert(0, nuevo);
        Seleccionado = nuevo;
        NotificarComandos();
    }

    private void Guardar()
    {
        if (Seleccionado == null || string.IsNullOrWhiteSpace(Seleccionado.NombreProducto))
        {
            MessageBox.Show("Seleccione un producto y complete el nombre.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var p = Seleccionado;
        if (_esNuevo)
        {
            p.ProductoID = _repo.Insertar(p);
            _esNuevo = false;
        }
        else
        {
            _repo.Actualizar(p);
        }

        Productos.Clear();
        foreach (var item in _repo.Listar()) Productos.Add(item);
        Seleccionado = Productos.FirstOrDefault(x => x.ProductoID == p.ProductoID);
        NotificarComandos();
    }

    private void Eliminar()
    {
        if (Seleccionado == null) return;
        if (MessageBox.Show("Desea eliminar el producto seleccionado?", "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        _repo.Eliminar(Seleccionado.ProductoID);
        Productos.Remove(Seleccionado);
        Seleccionado = null;
    }

    private void Cancelar()
    {
        if (_esNuevo && Productos.Count > 0) Productos.RemoveAt(0);
        _esNuevo = false;
        Seleccionado = null;
        NotificarComandos();
    }

    private void NotificarComandos()
    {
        OnPropertyChanged(nameof(Seleccionado));
        CommandManager.InvalidateRequerySuggested();
    }
}
