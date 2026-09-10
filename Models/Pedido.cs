using System.Collections.ObjectModel;

namespace NeptunoApp.Models;

public class Pedido
{
    public int PedidoID { get; set; }
    public int? ClienteID { get; set; }
    public int? EmpleadoID { get; set; }
    public DateTime FechaPedido { get; set; }
    public DateTime? FechaRequerida { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public int? TransportistaID { get; set; }
    public string? Destinatario { get; set; }
    public string? CiudadDestino { get; set; }
    public string? PaisDestino { get; set; }

    public string? NombreCliente { get; set; }
    public string? NombreEmpleado { get; set; }
    public string? NombreTransportista { get; set; }

    public ObservableCollection<DetalleLinea>? Detalle { get; set; }
}

public class DetalleLinea
{
    public int PedidoID { get; set; }
    public int ProductoID { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal PrecioUnidad { get; set; }
    public short Cantidad { get; set; }
    public decimal Descuento { get; set; }

    public DateTime FechaPedido { get; set; }
    public string? NombreCliente { get; set; }

    public decimal PrecioCantidad => PrecioUnidad * Cantidad;
    public decimal DescuentoMonto => PrecioCantidad * Descuento;
    public decimal TotalLinea => PrecioCantidad * (1m - Descuento);
}
