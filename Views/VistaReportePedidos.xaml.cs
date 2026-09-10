using System.Windows;

namespace NeptunoApp.Views;

public partial class VistaReportePedidos : Window
{
    public VistaReportePedidos(object viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
