using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeptunoApp.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? propiedad = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;
        campo = valor;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        return true;
    }

    protected void OnPropertyChanged(string nombre)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
}
