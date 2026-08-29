using System.Windows.Forms;

namespace Hospital.Presentacion
{
    /// <summary>
    /// Mensajes al usuario en un solo lugar.
    ///
    /// Centralizarlos evita que cada formulario elija por su cuenta el icono, el
    /// título y los botones, y deja un único punto donde cambiar la forma de
    /// notificar si mañana se sustituyen los MessageBox por avisos integrados.
    /// </summary>
    public static class Avisos
    {
        public static void Informacion(IWin32Window propietario, string mensaje, string titulo = "Información")
        {
            MessageBox.Show(propietario, mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>Reglas de negocio: el usuario puede corregir y reintentar.</summary>
        public static void Advertencia(IWin32Window propietario, string mensaje, string titulo = "Revise los datos")
        {
            MessageBox.Show(propietario, mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>Fallas técnicas: el detalle ya quedó en el log, aquí va el texto neutro.</summary>
        public static void Error(IWin32Window propietario, string mensaje, string titulo = "Error")
        {
            MessageBox.Show(propietario, mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static bool Confirmar(IWin32Window propietario, string mensaje, string titulo = "Confirmación")
        {
            return MessageBox.Show(propietario, mensaje, titulo,
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
