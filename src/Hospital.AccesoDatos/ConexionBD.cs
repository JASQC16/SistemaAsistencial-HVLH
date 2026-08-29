using System.Configuration;
using System.Data.SqlClient;
using Hospital.Utilidades;

namespace Hospital.AccesoDatos
{
    /// <summary>
    /// Punto único de creación de conexiones. La cadena vive en App.config, no en el
    /// código, para poder cambiar de servidor sin recompilar.
    /// </summary>
    internal static class ConexionBD
    {
        private const string NombreCadena = "HospitalDB";

        public static string CadenaConexion
        {
            get
            {
                var configuracion = ConfigurationManager.ConnectionStrings[NombreCadena];
                if (configuracion == null || string.IsNullOrWhiteSpace(configuracion.ConnectionString))
                {
                    throw new NegocioException(
                        "No se encontró la cadena de conexión '" + NombreCadena + "' en el archivo App.config.");
                }
                return configuracion.ConnectionString;
            }
        }

        public static SqlConnection Crear()
        {
            return new SqlConnection(CadenaConexion);
        }
    }
}
