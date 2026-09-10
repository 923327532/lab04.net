using System.Windows;
using System.Windows.Controls;

namespace NeptunoApp.Views;

public partial class VistaPedidos : UserControl
{
    public VistaPedidos()
    {
        InitializeComponent();
    }

    private void BtnReporte_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new VistaReportePedidos(DataContext);
        ventana.Owner = Window.GetWindow(this);
        ventana.ShowDialog();
    }
}
