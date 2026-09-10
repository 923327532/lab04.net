using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels;

public class CategoriaViewModel : ViewModelBase
{
    private readonly CategoriaRepositorio _repo;
    private Categoria? _seleccionada;
    private bool _esNueva;

    public ObservableCollection<Categoria> Categorias { get; } = new();

    public ICommand CargarCommand { get; }
    public ICommand NuevoCommand { get; }
    public ICommand GuardarCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand CancelarCommand { get; }

    public CategoriaViewModel()
    {
        _repo = new CategoriaRepositorio();
        CargarCommand = new RelayCommand(_ => Cargar());
        NuevoCommand = new RelayCommand(_ => Nueva(), _ => !_esNueva);
        GuardarCommand = new RelayCommand(_ => Guardar());
        EliminarCommand = new RelayCommand(_ => Eliminar(), _ => !_esNueva);
        CancelarCommand = new RelayCommand(_ => Cancelar(), _ => _esNueva);
    }

    public Categoria? Seleccionada
    {
        get => _seleccionada;
        set => SetProperty(ref _seleccionada, value);
    }

    public void Cargar()
    {
        Categorias.Clear();
        foreach (var c in _repo.Listar()) Categorias.Add(c);
    }

    private void Nueva()
    {
        _esNueva = true;
        var nueva = new Categoria { CategoriaID = 0, NombreCategoria = "Nueva categoria" };
        Categorias.Insert(0, nueva);
        Seleccionada = nueva;
        Notificar();
    }

    private void Guardar()
    {
        if (Seleccionada == null || string.IsNullOrWhiteSpace(Seleccionada.NombreCategoria))
        {
            MessageBox.Show("Seleccione una categoria y complete el nombre.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var c = Seleccionada;
        if (_esNueva)
        {
            c.CategoriaID = _repo.Insertar(c);
            _esNueva = false;
        }
        else
        {
            _repo.Actualizar(c);
        }
        Cargar();
        Seleccionada = Categorias.FirstOrDefault(x => x.CategoriaID == c.CategoriaID);
        Notificar();
        MessageBox.Show("Guardado exitoso.", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Eliminar()
    {
        if (Seleccionada == null) return;
        if (MessageBox.Show("Desea eliminar la categoria seleccionada?", "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;
        _repo.Eliminar(Seleccionada.CategoriaID);
        Categorias.Remove(Seleccionada);
        Seleccionada = null;
    }

    private void Cancelar()
    {
        if (_esNueva && Categorias.Count > 0) Categorias.RemoveAt(0);
        _esNueva = false;
        Seleccionada = null;
        OnPropertyChanged(nameof(Seleccionada));
        CommandManager.InvalidateRequerySuggested();
    }

    private void Notificar()
    {
        OnPropertyChanged(nameof(Seleccionada));
        CommandManager.InvalidateRequerySuggested();
    }
}