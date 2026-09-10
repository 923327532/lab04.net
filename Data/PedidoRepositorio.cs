using System.Data;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

public class PedidoRepositorio
{
    public List<Pedido> ListarPedidos()
    {
        var resultado = new List<Pedido>();
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spPedidos_Listar", cn) { CommandType = CommandType.StoredProcedure };
        cn.Open();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            var p = new Pedido
            {
                PedidoID = rd.GetInt32(rd.GetOrdinal("PedidoID")),
                ClienteID = NuloInt(rd, "ClienteID"),
                EmpleadoID = NuloInt(rd, "EmpleadoID"),
                FechaPedido = rd.GetDateTime(rd.GetOrdinal("FechaPedido")),
                FechaRequerida = NuloFecha(rd, "FechaRequerida"),
                FechaEnvio = NuloFecha(rd, "FechaEnvio"),
                TransportistaID = NuloInt(rd, "TransportistaID"),
                Destinatario = NuloStr(rd, "Destinatario"),
                CiudadDestino = NuloStr(rd, "CiudadDestino"),
                PaisDestino = NuloStr(rd, "PaisDestino"),
                NombreCliente = NuloStr(rd, "NombreCliente"),
                NombreEmpleado = NuloStr(rd, "NombreEmpleado"),
                NombreTransportista = NuloStr(rd, "NombreTransportista")
            };
            resultado.Add(p);
        }
        return resultado;
    }

    public int InsertarPedido(Pedido p)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spPedidos_Insertar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@ClienteID", (object?)p.ClienteID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@EmpleadoID", (object?)p.EmpleadoID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaPedido", p.FechaPedido);
        cmd.Parameters.AddWithValue("@FechaRequerida", (object?)p.FechaRequerida ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaEnvio", (object?)p.FechaEnvio ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TransportistaID", (object?)p.TransportistaID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Destinatario", (object?)p.Destinatario ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CiudadDestino", (object?)p.CiudadDestino ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PaisDestino", (object?)p.PaisDestino ?? DBNull.Value);
        cn.Open();
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void ActualizarPedido(Pedido p)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spPedidos_Actualizar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@PedidoID", p.PedidoID);
        cmd.Parameters.AddWithValue("@ClienteID", (object?)p.ClienteID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@EmpleadoID", (object?)p.EmpleadoID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaPedido", p.FechaPedido);
        cmd.Parameters.AddWithValue("@FechaRequerida", (object?)p.FechaRequerida ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaEnvio", (object?)p.FechaEnvio ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TransportistaID", (object?)p.TransportistaID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Destinatario", (object?)p.Destinatario ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CiudadDestino", (object?)p.CiudadDestino ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PaisDestino", (object?)p.PaisDestino ?? DBNull.Value);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    public void EliminarPedido(int pedidoID)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spPedidos_Eliminar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@PedidoID", pedidoID);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    public ObservableCollection<DetalleLinea> ListarDetalle(int pedidoID)
    {
        var resultado = new ObservableCollection<DetalleLinea>();
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spDetallePedidos_Listar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@PedidoID", pedidoID);
        cn.Open();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            resultado.Add(new DetalleLinea
            {
                PedidoID = rd.GetInt32(rd.GetOrdinal("PedidoID")),
                ProductoID = rd.GetInt32(rd.GetOrdinal("ProductoID")),
                NombreProducto = rd.GetString(rd.GetOrdinal("NombreProducto")),
                PrecioUnidad = rd.GetDecimal(rd.GetOrdinal("PrecioUnidad")),
                Cantidad = rd.GetInt16(rd.GetOrdinal("Cantidad")),
                Descuento = rd.GetDecimal(rd.GetOrdinal("Descuento"))
            });
        }
        return resultado;
    }


    public void AgregarDetalle(DetalleLinea linea)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spDetallePedidos_Insertar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@PedidoID", linea.PedidoID);
        cmd.Parameters.AddWithValue("@ProductoID", linea.ProductoID);
        cmd.Parameters.AddWithValue("@PrecioUnidad", linea.PrecioUnidad);
        cmd.Parameters.AddWithValue("@Cantidad", linea.Cantidad);
        cmd.Parameters.AddWithValue("@Descuento", linea.Descuento);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    public void QuitarDetalle(int pedidoID, int productoID)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spDetallePedidos_Eliminar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@PedidoID", pedidoID);
        cmd.Parameters.AddWithValue("@ProductoID", productoID);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    public List<DetalleLinea> ListarReporteFechas(DateTime? inicio, DateTime? fin)
    {
        var resultado = new List<DetalleLinea>();
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spDetallePedidos_ListarFechas", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@FechaInicio", (object?)inicio ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaFin", (object?)fin ?? DBNull.Value);
        cn.Open();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            resultado.Add(new DetalleLinea
            {
                PedidoID = rd.GetInt32(rd.GetOrdinal("PedidoID")),
                FechaPedido = rd.GetDateTime(rd.GetOrdinal("FechaPedido")),
                ProductoID = rd.GetInt32(rd.GetOrdinal("ProductoID")),
                NombreProducto = rd.GetString(rd.GetOrdinal("NombreProducto")),
                PrecioUnidad = rd.GetDecimal(rd.GetOrdinal("PrecioUnidad")),
                Cantidad = rd.GetInt16(rd.GetOrdinal("Cantidad")),
                Descuento = rd.GetDecimal(rd.GetOrdinal("Descuento")),
                NombreCliente = NuloStr(rd, "NombreCliente")
            });
        }
        return resultado;
    }

    private static int? NuloInt(SqlDataReader rd, string col)
        => rd.IsDBNull(rd.GetOrdinal(col)) ? null : rd.GetInt32(rd.GetOrdinal(col));

    private static string? NuloStr(SqlDataReader rd, string col)
        => rd.IsDBNull(rd.GetOrdinal(col)) ? null : rd.GetString(rd.GetOrdinal(col));

    private static DateTime? NuloFecha(SqlDataReader rd, string col)
        => rd.IsDBNull(rd.GetOrdinal(col)) ? null : rd.GetDateTime(rd.GetOrdinal(col));
}

