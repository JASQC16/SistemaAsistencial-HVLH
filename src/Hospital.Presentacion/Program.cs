using System;
using System.Threading;
using System.Windows.Forms;
using Hospital.Presentacion.Formularios;
using Hospital.Utilidades;

namespace Hospital.Presentacion
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Red de seguridad: cualquier excepción no controlada se registra y se
            // informa al usuario en lugar de cerrar la aplicación de forma abrupta.
            Application.ThreadException += (s, e) => ManejarErrorGlobal(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => ManejarErrorGlobal(e.ExceptionObject as Exception);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Permite cerrar sesión y volver al login sin reiniciar la aplicación.
            while (true)
            {
                using (var login = new FrmLogin())
                {
                    if (login.ShowDialog() != DialogResult.OK) break;
                }

                Application.Run(new FrmPrincipal());

                if (!FrmPrincipal.ReiniciarSesion) break;
                Sesion.Cerrar();
            }
        }

        private static void ManejarErrorGlobal(Exception ex)
        {
            if (ex == null) return;
            Registro.Error("Excepción no controlada", ex);
            MessageBox.Show(
                "Ocurrió un error inesperado y la operación no pudo completarse.\n\n" +
                "El detalle técnico se guardó en la carpeta Logs de la aplicación.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
