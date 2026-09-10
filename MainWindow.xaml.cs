using System.Windows;
using NeptunoApp.ViewModels;

namespace NeptunoApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
