using System;
using System.IO;
using System.Text;

namespace Hospital.Utilidades
{
    /// <summary>Registro de errores en archivo de texto. Nunca lanza excepciones propias.</summary>
    public static class Registro
    {
        private static readonly object Candado = new object();

        private static string RutaArchivo
        {
            get
            {
                string carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
                return Path.Combine(carpeta, "errores_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            }
        }

        public static void Error(string origen, Exception ex)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("-----------------------------------------------------------");
                sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  [" + origen + "]");
                sb.AppendLine(ex.ToString());
                lock (Candado)
                {
                    File.AppendAllText(RutaArchivo, sb.ToString(), Encoding.UTF8);
                }
            }
            catch
            {
                // El registro de errores nunca debe interrumpir la operación del usuario.
            }
        }

        public static void Informacion(string mensaje)
        {
            try
            {
                lock (Candado)
                {
                    File.AppendAllText(RutaArchivo,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  [INFO] " + mensaje + Environment.NewLine,
                        Encoding.UTF8);
                }
            }
            catch { }
        }
    }
}
