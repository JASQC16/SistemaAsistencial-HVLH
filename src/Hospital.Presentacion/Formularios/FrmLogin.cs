using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Hospital.Entidades;
using Hospital.Negocio;
using Hospital.Utilidades;

namespace Hospital.Presentacion.Formularios
{
    public partial class FrmLogin : Form
    {
        private readonly UsuarioServicio _servicio = new UsuarioServicio();

        public FrmLogin()
        {
            InitializeComponent();

            // El nombre viaja por configuración para no recompilar si cambia la
            // denominación oficial; el valor del archivo es la fuente de verdad.
            lblInstitucion.Text = ConfigurationManager.AppSettings["NombreInstitucion"] ?? Tema.NombreHospital;
            picLogo.Image = Recursos.Logo;
        }

        /// <summary>Franja azul institucional al pie del encabezado.</summary>
        private void PnlEncabezado_Paint(object sender, PaintEventArgs e)
        {
            using (var lapiz = new Pen(Tema.Azul, 3))
            {
                e.Graphics.DrawLine(lapiz, 0, pnlEncabezado.Height - 2, pnlEncabezado.Width, pnlEncabezado.Height - 2);
            }
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            Ingresar();
        }

        private void Ingresar()
        {
            lblMensaje.Text = string.Empty;
            Cursor = Cursors.WaitCursor;
            btnIngresar.Enabled = false;

            try
            {
                Usuario usuario = _servicio.Autenticar(txtUsuario.Text, txtClave.Text);
                Sesion.Iniciar(usuario.IdUsuario, usuario.NombreUsuario, usuario.NombreCompleto, usuario.Rol);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (NegocioException ex)
            {
                MostrarError(ex.Message);
                txtClave.Clear();
                txtClave.Focus();
            }
            catch (DatosException ex)
            {
                MostrarError(ex.Message + " Verifique que SQL Server esté disponible.");
            }
            finally
            {
                btnIngresar.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void MostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Campo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Ingresar();
            }
        }
    }
}
