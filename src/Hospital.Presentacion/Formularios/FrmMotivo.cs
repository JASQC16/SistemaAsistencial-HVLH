using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hospital.Presentacion.Formularios
{
    /// <summary>
    /// Diálogo breve para capturar el motivo de anulación. Se construye por código
    /// por ser un componente pequeño y de un solo uso.
    /// </summary>
    public class FrmMotivo : Form
    {
        private readonly TextBox _txtMotivo;

        private FrmMotivo(string numeroAtencion)
        {
            var lblTitulo = new Label
            {
                Text = "Motivo de anulación de la atención " + numeroAtencion,
                Font = Tema.FuenteEtiqueta,
                Location = new Point(18, 18),
                Size = new Size(430, 22),
                ForeColor = Tema.Texto
            };

            var lblAyuda = new Label
            {
                Text = "Queda registrado en la historia clínica junto con el usuario responsable.",
                Location = new Point(18, 40),
                Size = new Size(430, 20),
                ForeColor = Tema.TextoSuave
            };

            _txtMotivo = new TextBox
            {
                Location = new Point(18, 66),
                Size = new Size(430, 70),
                Multiline = true,
                MaxLength = 200,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnAceptar = new Button { Text = "Anular atención", Location = new Point(258, 150), Size = new Size(130, 34) };
            var btnCancelar = new Button { Text = "Cancelar", Location = new Point(396, 150), Size = new Size(90, 34) };

            btnAceptar.Click += (s, e) =>
            {
                if (_txtMotivo.Text.Trim().Length < 5)
                {
                    MessageBox.Show("Describa el motivo con al menos 5 caracteres.", "Motivo requerido",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtMotivo.Focus();
                    return;
                }
                DialogResult = DialogResult.OK;
            };
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; };

            Controls.AddRange(new Control[] { lblTitulo, lblAyuda, _txtMotivo, btnAceptar, btnCancelar });

            ClientSize = new Size(504, 200);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Text = "Anular atención";
            AcceptButton = btnAceptar;
            CancelButton = btnCancelar;

            Tema.AplicarFormulario(this);
            Tema.CampoTexto(_txtMotivo);
            Tema.BotonPeligro(btnAceptar);
            Tema.BotonSecundario(btnCancelar);
        }

        /// <summary>Devuelve el motivo ingresado, o null si el usuario canceló.</summary>
        public static string Solicitar(IWin32Window propietario, string numeroAtencion)
        {
            using (var dialogo = new FrmMotivo(numeroAtencion))
            {
                return dialogo.ShowDialog(propietario) == DialogResult.OK
                    ? dialogo._txtMotivo.Text.Trim()
                    : null;
            }
        }
    }
}
