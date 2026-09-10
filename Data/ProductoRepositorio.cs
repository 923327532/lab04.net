using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

public class ProductoRepositorio
{
    public List<Producto> Listar()
    {
        var resultado = new List<Producto>();
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spProductos_Listar", cn) { CommandType = CommandType.StoredProcedure };
        cn.Open();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            resultado.Add(new Producto
            {
                ProductoID = rd.GetInt32(rd.GetOrdinal("ProductoID")),
                NombreProducto = rd.GetString(rd.GetOrdinal("NombreProducto")),
                ProveedorID = GetNuloInt(rd, "ProveedorID"),
                CategoriaID = GetNuloInt(rd, "CategoriaID"),
                CantidadPorUnidad = GetNuloStr(rd, "CantidadPorUnidad"),
                PrecioUnidad = rd.GetDecimal(rd.GetOrdinal("PrecioUnidad")),
                UnidadesEnExistencia = rd.GetInt16(rd.GetOrdinal("UnidadesEnExistencia")),
                UnidadesEnPedido = rd.GetInt16(rd.GetOrdinal("UnidadesEnPedido")),
                NivelDeReorden = rd.GetInt16(rd.GetOrdinal("NivelDeReorden")),
                Descontinuado = rd.GetBoolean(rd.GetOrdinal("Descontinuado")),
                NombreCategoria = GetNuloStr(rd, "NombreCategoria"),
                CompaniaNombre = GetNuloStr(rd, "CompaniaNombre")
            });
        }
        return resultado;
    }

    public int Insertar(Producto p)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spProductos_Insertar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@NombreProducto", p.NombreProducto);
        cmd.Parameters.AddWithValue("@ProveedorID", (object?)p.ProveedorID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CategoriaID", (object?)p.CategoriaID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CantidadPorUnidad", (object?)p.CantidadPorUnidad ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PrecioUnidad", p.PrecioUnidad);
        cmd.Parameters.AddWithValue("@UnidadesEnExistencia", p.UnidadesEnExistencia);
        cmd.Parameters.AddWithValue("@UnidadesEnPedido", p.UnidadesEnPedido);
        cmd.Parameters.AddWithValue("@NivelDeReorden", p.NivelDeReorden);
        cmd.Parameters.AddWithValue("@Descontinuado", p.Descontinuado);
        cn.Open();
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Actualizar(Producto p)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spProductos_Actualizar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@ProductoID", p.ProductoID);
        cmd.Parameters.AddWithValue("@NombreProducto", p.NombreProducto);
        cmd.Parameters.AddWithValue("@ProveedorID", (object?)p.ProveedorID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CategoriaID", (object?)p.CategoriaID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CantidadPorUnidad", (object?)p.CantidadPorUnidad ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PrecioUnidad", p.PrecioUnidad);
        cmd.Parameters.AddWithValue("@UnidadesEnExistencia", p.UnidadesEnExistencia);
        cmd.Parameters.AddWithValue("@UnidadesEnPedido", p.UnidadesEnPedido);
        cmd.Parameters.AddWithValue("@NivelDeReorden", p.NivelDeReorden);
        cmd.Parameters.AddWithValue("@Descontinuado", p.Descontinuado);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    public void Eliminar(int productoID)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spProductos_Eliminar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@ProductoID", productoID);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    private static int? GetNuloInt(SqlDataReader rd, string col)
        => rd.IsDBNull(rd.GetOrdinal(col)) ? null : rd.GetInt32(rd.GetOrdinal(col));

    private static string? GetNuloStr(SqlDataReader rd, string col)
        => rd.IsDBNull(rd.GetOrdinal(col)) ? null : rd.GetString(rd.GetOrdinal(col));
}
