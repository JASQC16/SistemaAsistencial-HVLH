using System.Drawing;
using System.Windows.Forms;

namespace Hospital.Presentacion.Formularios
{
    /// <summary>
    /// Diálogo breve para capturar un texto obligatorio: el motivo de una
    /// cancelación, de una inasistencia o de un paciente no atendido.
    ///
    /// Se construye por código por ser un componente pequeño y sin estado propio.
    /// Es genérico a propósito: tener un diálogo distinto por cada motivo terminaría
    /// en tres formularios idénticos salvo por el título.
    /// </summary>
    public class FrmTextoBreve : Form
    {
        private readonly TextBox _txtTexto;

        private FrmTextoBreve(string titulo, string contexto, string indicacion)
        {
            var lblContexto = new Label
            {
                Text = contexto,
                Font = Tema.FuenteEtiqueta,
                ForeColor = Tema.AzulProfundo,
                Location = new Point(18, 18),
                Size = new Size(450, 22)
            };

            var lblIndicacion = new Label
            {
                Text = indicacion,
                ForeColor = Tema.TextoSuave,
                Location = new Point(18, 42),
                Size = new Size(450, 34)
            };

            _txtTexto = new TextBox
            {
                Location = new Point(18, 80),
                Size = new Size(450, 74),
                Multiline = true,
                MaxLength = 300,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnAceptar = new Button { Text = "Registrar", Location = new Point(268, 168), Size = new Size(110, 34) };
            var btnCancelar = new Button { Text = "Cancelar", Location = new Point(388, 168), Size = new Size(90, 34) };

            btnAceptar.Click += (remitente, argumentos) =>
            {
                if (_txtTexto.Text.Trim().Length < 5)
                {
                    Avisos.Advertencia(this, "Describa el motivo con al menos 5 caracteres.", "Motivo requerido");
                    _txtTexto.Focus();
                    return;
                }
                DialogResult = DialogResult.OK;
            };
            btnCancelar.Click += (remitente, argumentos) => { DialogResult = DialogResult.Cancel; };

            Controls.AddRange(new Control[] { lblContexto, lblIndicacion, _txtTexto, btnAceptar, btnCancelar });

            ClientSize = new Size(496, 218);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Text = "HVLH — " + titulo;
            AcceptButton = btnAceptar;
            CancelButton = btnCancelar;

            Tema.AplicarFormulario(this);
            Tema.CampoTexto(_txtTexto);
            Tema.BotonPrimario(btnAceptar);
            Tema.BotonSecundario(btnCancelar);
        }

        /// <summary>Devuelve el texto ingresado, o null si el usuario canceló.</summary>
        public static string Solicitar(IWin32Window propietario, string titulo, string contexto, string indicacion)
        {
            using (var dialogo = new FrmTextoBreve(titulo, contexto, indicacion))
            {
                return dialogo.ShowDialog(propietario) == DialogResult.OK
                    ? dialogo._txtTexto.Text.Trim()
                    : null;
            }
        }
    }
}
