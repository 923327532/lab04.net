using Microsoft.Data.SqlClient;

namespace NeptunoApp.Data;

public static class ConexionBD
{
    public const string StringConexion =
        "Server=.\\SQLEXPRESS;Database=NeptunoDB;Integrated Security=True;TrustServerCertificate=True;";

    public static SqlConnection Crear() => new SqlConnection(StringConexion);
}
