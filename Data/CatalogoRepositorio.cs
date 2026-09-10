using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

public class CatalogoRepositorio
{
    public List<Categoria> Categorias()
        => EjecutarCatalogos("dbo.spCatalogos_Categorias",
            r => new Categoria { CategoriaID = r.GetInt32(0), NombreCategoria = r.GetString(1) });

    public List<Proveedor> Proveedores()
        => EjecutarCatalogos("dbo.spCatalogos_Proveedores",
            r => new Proveedor { ProveedorID = r.GetInt32(0), CompaniaNombre = r.GetString(1) });

    public List<Cliente> Clientes()
        => EjecutarCatalogos("dbo.spCatalogos_Clientes",
            r => new Cliente { ClienteID = r.GetInt32(0), Empresa = r.GetString(1) });

    public List<Empleado> Empleados()
        => EjecutarCatalogos("dbo.spCatalogos_Empleados",
            r => new Empleado { EmpleadoID = r.GetInt32(0), NombreCompleto = r.GetString(1) });

    public List<Transportista> Transportistas()
        => EjecutarCatalogos("dbo.spCatalogos_Transportistas",
            r => new Transportista { TransportistaID = r.GetInt32(0), CompaniaNombre = r.GetString(1) });

    public List<Producto> Productos()
        => EjecutarCatalogos("dbo.spCatalogos_Productos",
            r => new Producto { ProductoID = r.GetInt32(0), NombreProducto = r.GetString(1), PrecioUnidad = r.GetDecimal(2) });

    private List<T> EjecutarCatalogos<T>(string nombreProc, Func<SqlDataReader, T> mapear)
    {
        var resultado = new List<T>();
        using var cn = ConexionBD.Crear();
        using var cmd = new SqlCommand(nombreProc, cn) { CommandType = CommandType.StoredProcedure };
        cn.Open();
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) resultado.Add(mapear(rd));
        return resultado;
    }
}
