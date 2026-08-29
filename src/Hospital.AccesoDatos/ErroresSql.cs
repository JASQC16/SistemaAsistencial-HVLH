using System;
using System.Data.SqlClient;
using Hospital.Utilidades;

namespace Hospital.AccesoDatos
{
    /// <summary>
    /// Traducción de errores de SQL Server a excepciones de la aplicación.
    ///
    /// Vive en un solo lugar para que todos los repositorios clasifiquen igual: un
    /// error lanzado con THROW desde un procedimiento (número >= 50000) es una regla
    /// de negocio y su mensaje se muestra al usuario tal cual; el resto es una falla
    /// técnica, se registra en el log y el usuario recibe un texto neutro.
    /// </summary>
    internal static class ErroresSql
    {
        public static Exception Traducir(SqlException ex, string mensajeGenerico)
        {
            if (ex.Number >= 50000) return new NegocioException(ex.Message, ex);

            switch (ex.Number)
            {
                case 2627:   // violación de PK o UNIQUE
                case 2601:   // violación de índice único
                    return new NegocioException(
                        "Ya existe un registro con los mismos datos clave. Verifique que no esté duplicando la información.", ex);

                case 547:    // violación de FK o CHECK
                    return new NegocioException(
                        "Los datos ingresados no cumplen una restricción de integridad de la base de datos.", ex);

                case -2:     // tiempo de espera agotado
                    return new DatosException("La base de datos no respondió en el tiempo esperado.", ex);

                case 4060:
                case 18456:  // no se pudo abrir la base o credenciales inválidas
                    return new DatosException(
                        "No fue posible conectarse a la base de datos. Revise la cadena de conexión en App.config.", ex);

                default:
                    return new DatosException(mensajeGenerico, ex);
            }
        }

        /// <summary>Revierte una transacción sin dejar que un fallo al revertir oculte el error original.</summary>
        public static void Revertir(SqlTransaction transaccion, string origen)
        {
            try
            {
                if (transaccion != null && transaccion.Connection != null) transaccion.Rollback();
            }
            catch (Exception ex)
            {
                Registro.Error(origen + " (rollback)", ex);
            }
        }
    }
}
