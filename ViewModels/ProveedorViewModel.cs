using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels;

public class ProveedorViewModel : ViewModelBase
{
    private readonly ProveedorRepositorio _repo;

    private Proveedor? _seleccionado;
    private bool _esNuevo;
    private string _filtroNombre = string.Empty;
    private string _filtroCiudad = string.Empty;
    private string _mensaje = string.Empty;

    public ObservableCollection<Proveedor> Proveedores { get; } = new();

    public ICommand CargarCommand { get; }
    public ICommand BuscarCommand { get; }
    public ICommand NuevoCommand { get; }
    public ICommand GuardarCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand CancelarCommand { get; }

    public ProveedorViewModel()
    {
        _repo = new ProveedorRepositorio();
        CargarCommand = new RelayCommand(_ => Cargar());
        BuscarCommand = new RelayCommand(_ => Buscar());
        NuevoCommand = new RelayCommand(_ => Nuevo(), _ => !_esNuevo);
        GuardarCommand = new RelayCommand(_ => Guardar());
        EliminarCommand = new RelayCommand(_ => Eliminar(), _ => !_esNuevo);
        CancelarCommand = new RelayCommand(_ => Cancelar(), _ => _esNuevo);
    }

    public Proveedor? Seleccionado
    {
        get => _seleccionado;
        set => SetProperty(ref _seleccionado, value);
    }

    public string FiltroNombre
    {
        get => _filtroNombre;
        set => SetProperty(ref _filtroNombre, value);
    }

    public string FiltroCiudad
    {
        get => _filtroCiudad;
        set => SetProperty(ref _filtroCiudad, value);
    }

    public string Mensaje
    {
        get => _mensaje;
        set => SetProperty(ref _mensaje, value);
    }

    public void Cargar()
    {
        Proveedores.Clear();
        var lista = string.IsNullOrWhiteSpace(FiltroNombre) && string.IsNullOrWhiteSpace(FiltroCiudad)
            ? _repo.Listar()
            : _repo.Buscar(TextoNulo(FiltroNombre), TextoNulo(FiltroCiudad));
        foreach (var p in lista) Proveedores.Add(p);
        Mensaje = $"{Proveedores.Count} proveedores";
    }

    private void Buscar()
    {
        Cargar();
        Notificar();
    }

    private void Nuevo()
    {
        _esNuevo = true;
        var nuevo = new Proveedor { ProveedorID = 0, CompaniaNombre = "Nuevo proveedor" };
        Proveedores.Insert(0, nuevo);
        Seleccionado = nuevo;
        Notificar();
    }

    private void Guardar()
    {
        if (Seleccionado == null || string.IsNullOrWhiteSpace(Seleccionado.CompaniaNombre))
        {
            MessageBox.Show("Seleccione un proveedor y complete el nombre de la compania.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var p = Seleccionado;
        if (_esNuevo)
        {
            p.ProveedorID = _repo.Insertar(p);
            _esNuevo = false;
        }
        else
        {
            _repo.Actualizar(p);
        }
        Cargar();
        Seleccionado = Proveedores.FirstOrDefault(x => x.ProveedorID == p.ProveedorID);
        Notificar();
    }

    private void Eliminar()
    {
        if (Seleccionado == null) return;
        if (MessageBox.Show("Desea eliminar el proveedor seleccionado?", "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;
        _repo.Eliminar(Seleccionado.ProveedorID);
        Proveedores.Remove(Seleccionado);
        Seleccionado = null;
        MantenerFiltro();
    }

    private void Cancelar()
    {
        if (_esNuevo && Proveedores.Count > 0) Proveedores.RemoveAt(0);
        _esNuevo = false;
        Seleccionado = null;
        Notificar();
    }

    private void MantenerFiltro()
    {
        Mensaje = $"{Proveedores.Count} proveedores";
    }

    private static string? TextoNulo(string texto) => string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();

    private void Notificar()
    {
        OnPropertyChanged(nameof(Seleccionado));
        CommandManager.InvalidateRequerySuggested();
    }
}
