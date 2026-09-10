using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

public class ProveedorRepositorio
{
    public List<Proveedor> Listar()
    {
        var resultado = new List<Proveedor>();
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spProveedores_Listar", cn) { CommandType = CommandType.StoredProcedure };
        cn.Open();
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) resultado.Add(Mapear(rd));
        return resultado;
    }

    public List<Proveedor> Buscar(string? nombreContacto, string? ciudad)
    {
        var resultado = new List<Proveedor>();
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spProveedores_Buscar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@NombreContacto", (object?)nombreContacto ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Ciudad", (object?)ciudad ?? DBNull.Value);
        cn.Open();
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) resultado.Add(Mapear(rd));
        return resultado;
    }

    public int Insertar(Proveedor p)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spProveedores_Insertar", cn) { CommandType = CommandType.StoredProcedure };
        AgregarParametros(cmd, p, incluirID: false);
        cn.Open();
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Actualizar(Proveedor p)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spProveedores_Actualizar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@ProveedorID", p.ProveedorID);
        AgregarParametros(cmd, p, incluirID: false);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    public void Eliminar(int proveedorID)
    {
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand("dbo.spProveedores_Eliminar", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    private static void AgregarParametros(SqlCommand cmd, Proveedor p, bool incluirID)
    {
        if (incluirID) cmd.Parameters.AddWithValue("@ProveedorID", p.ProveedorID);
        cmd.Parameters.AddWithValue("@CompaniaNombre", p.CompaniaNombre);
        cmd.Parameters.AddWithValue("@NombreContacto", ArgumentoNulo(p.NombreContacto));
        cmd.Parameters.AddWithValue("@CargoContacto", ArgumentoNulo(p.CargoContacto));
        cmd.Parameters.AddWithValue("@Direccion", ArgumentoNulo(p.Direccion));
        cmd.Parameters.AddWithValue("@Ciudad", ArgumentoNulo(p.Ciudad));
        cmd.Parameters.AddWithValue("@CodigoPostal", ArgumentoNulo(p.CodigoPostal));
        cmd.Parameters.AddWithValue("@Pais", ArgumentoNulo(p.Pais));
        cmd.Parameters.AddWithValue("@Telefono", ArgumentoNulo(p.Telefono));
        cmd.Parameters.AddWithValue("@Fax", ArgumentoNulo(p.Fax));
    }

    private static object ArgumentoNulo(string? valor) => (object?)valor ?? DBNull.Value;

    private static Proveedor Mapear(SqlDataReader rd)
    {
        return new Proveedor
        {
            ProveedorID = rd.GetInt32(rd.GetOrdinal("ProveedorID")),
            CompaniaNombre = rd.GetString(rd.GetOrdinal("CompaniaNombre")),
            NombreContacto = GetNulo(rd, "NombreContacto"),
            CargoContacto = GetNulo(rd, "CargoContacto"),
            Direccion = GetNulo(rd, "Direccion"),
            Ciudad = GetNulo(rd, "Ciudad"),
            CodigoPostal = GetNulo(rd, "CodigoPostal"),
            Pais = GetNulo(rd, "Pais"),
            Telefono = GetNulo(rd, "Telefono"),
            Fax = GetNulo(rd, "Fax")
        };
    }

    private static string? GetNulo(SqlDataReader rd, string col)
        => rd.IsDBNull(rd.GetOrdinal(col)) ? null : rd.GetString(rd.GetOrdinal(col));
}
