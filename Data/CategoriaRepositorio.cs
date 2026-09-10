using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

public class CategoriaRepositorio
{
    public List<Categoria> Listar()
    {
        var resultado = new List<Categoria>();
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spCategorias_Listar", cn) { CommandType = CommandType.StoredProcedure };
        cn.Open();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            resultado.Add(new Categoria
            {
                CategoriaID = rd.GetInt32(rd.GetOrdinal("CategoriaID")),
                NombreCategoria = rd.GetString(rd.GetOrdinal("NombreCategoria")),
                Descripcion = rd.IsDBNull(rd.GetOrdinal("Descripcion")) ? null : rd.GetString(rd.GetOrdinal("Descripcion"))
            });
        }
        return resultado;
    }

    public int Insertar(Categoria c)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spCategorias_Insertar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@NombreCategoria", c.NombreCategoria);
        cmd.Parameters.AddWithValue("@Descripcion", (object?)c.Descripcion ?? DBNull.Value);
        cn.Open();
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Actualizar(Categoria c)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spCategorias_Actualizar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@CategoriaID", c.CategoriaID);
        cmd.Parameters.AddWithValue("@NombreCategoria", c.NombreCategoria);
        cmd.Parameters.AddWithValue("@Descripcion", (object?)c.Descripcion ?? DBNull.Value);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    public void Eliminar(int categoriaID)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spCategorias_Eliminar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@CategoriaID", categoriaID);
        cn.Open();
        cmd.ExecuteNonQuery();
    }
}
