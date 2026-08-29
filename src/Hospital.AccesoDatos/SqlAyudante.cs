using System;
using System.Data;
using System.Data.SqlClient;

namespace Hospital.AccesoDatos
{
    /// <summary>
    /// Utilidades comunes de ADO.NET. Todas las llamadas usan CommandType.StoredProcedure
    /// y parámetros tipados: nunca se concatena SQL, lo que elimina la inyección SQL.
    /// </summary>
    internal static class SqlAyudante
    {
        public static SqlCommand CrearComando(string nombreProcedimiento, SqlConnection conexion, SqlTransaction transaccion = null)
        {
            var comando = new SqlCommand(nombreProcedimiento, conexion)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 60
            };
            if (transaccion != null) comando.Transaction = transaccion;
            return comando;
        }

        /// <summary>Agrega un parámetro convirtiendo null y cadenas vacías a DBNull.</summary>
        public static void Agregar(SqlCommand comando, string nombre, SqlDbType tipo, object valor)
        {
            var parametro = comando.Parameters.Add(nombre, tipo);
            var texto = valor as string;
            parametro.Value = (valor == null || (texto != null && texto.Length == 0)) ? DBNull.Value : valor;
        }

        public static string LeerTexto(IDataRecord registro, string columna)
        {
            int indice = registro.GetOrdinal(columna);
            return registro.IsDBNull(indice) ? null : registro.GetString(indice);
        }

        public static int LeerEntero(IDataRecord registro, string columna)
        {
            int indice = registro.GetOrdinal(columna);
            return registro.IsDBNull(indice) ? 0 : Convert.ToInt32(registro.GetValue(indice));
        }

        public static int? LeerEnteroNulable(IDataRecord registro, string columna)
        {
            int indice = registro.GetOrdinal(columna);
            return registro.IsDBNull(indice) ? (int?)null : Convert.ToInt32(registro.GetValue(indice));
        }

        public static decimal? LeerDecimalNulable(IDataRecord registro, string columna)
        {
            int indice = registro.GetOrdinal(columna);
            return registro.IsDBNull(indice) ? (decimal?)null : Convert.ToDecimal(registro.GetValue(indice));
        }

        public static DateTime LeerFecha(IDataRecord registro, string columna)
        {
            int indice = registro.GetOrdinal(columna);
            return registro.IsDBNull(indice) ? DateTime.MinValue : Convert.ToDateTime(registro.GetValue(indice));
        }

        public static DateTime? LeerFechaNulable(IDataRecord registro, string columna)
        {
            int indice = registro.GetOrdinal(columna);
            return registro.IsDBNull(indice) ? (DateTime?)null : Convert.ToDateTime(registro.GetValue(indice));
        }

        public static bool LeerBooleano(IDataRecord registro, string columna)
        {
            int indice = registro.GetOrdinal(columna);
            return !registro.IsDBNull(indice) && Convert.ToBoolean(registro.GetValue(indice));
        }
    }
}
