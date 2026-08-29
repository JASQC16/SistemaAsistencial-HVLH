using System;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Hospital.Utilidades;

namespace Hospital.Presentacion.Formularios
{
    /// <summary>
    /// Ventana contenedora (MDI) del sistema.
    ///
    /// Concentra la navegación entre los cuatro módulos: pacientes, citas,
    /// atenciones y reportes. Cada uno es accesible por dos caminos —el menú y la
    /// barra de accesos directos— porque una opción que solo existe dentro de un
    /// submenú tiende a no encontrarse.
    /// </summary>
    public partial class FrmPrincipal : Form
    {
        /// <summary>Indica a Program.Main si debe volver a mostrar el login al cerrar.</summary>
        public static bool ReiniciarSesion { get; private set; }

        public FrmPrincipal()
        {
            InitializeComponent();
            ReiniciarSesion = false;

            string institucion = ConfigurationManager.AppSettings["NombreInstitucion"] ?? Tema.NombreHospitalCorto;
            Text = institucion + " — Sistema de atenciones";
            lblInstitucion.Text = institucion;
            picLogo.Image = Recursos.Logo;

            lblUsuario.Text = "Usuario: " + Sesion.NombreCompleto + "  (" + Sesion.Rol.ToLower() + ")";
            lblFecha.Text = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy");

            AplicarPermisos();
        }

        /// <summary>
        /// Habilita las opciones según el rol.
        ///
        /// El administrador tiene acceso a todo sin excepción, incluido el módulo de
        /// pacientes. El personal asistencial registra pacientes y agenda, pero no
        /// firma atenciones clínicas; esa restricción se aplica dentro del propio
        /// módulo de atenciones, no ocultando la opción, para que quede claro qué
        /// existe en el sistema y por qué no está disponible.
        /// </summary>
        private void AplicarPermisos()
        {
            bool administrador = Sesion.EsAdministrador;

            // Ningún módulo se oculta al administrador bajo ninguna circunstancia.
            mnuPacientes.Enabled = btnPacientes.Enabled = true;
            mnuCitas.Enabled = btnCitas.Enabled = true;
            mnuAtenciones.Enabled = btnAtenciones.Enabled = true;
            mnuReporteGeneral.Enabled = btnReportes.Enabled = true;

            if (administrador) lblModulo.Text = "Sistema de atenciones ambulatorias  ·  perfil administrador";
        }

        /// <summary>Franja azul institucional bajo el encabezado.</summary>
        private void PnlEncabezado_Paint(object sender, PaintEventArgs e)
        {
            using (var lapiz = new Pen(Tema.Azul, 3))
            {
                e.Graphics.DrawLine(lapiz, 0, pnlEncabezado.Height - 2, pnlEncabezado.Width, pnlEncabezado.Height - 2);
            }
        }

        // -------------------------------------------------------------------
        // Navegación
        // -------------------------------------------------------------------

        private void MnuPacientes_Click(object sender, EventArgs e)
        {
            AbrirHijo(new FrmPacientes());
        }

        private void MnuCitas_Click(object sender, EventArgs e)
        {
            AbrirHijo(new FrmCitas());
        }

        private void MnuAtenciones_Click(object sender, EventArgs e)
        {
            AbrirHijo(new FrmAtenciones());
        }

        private void MnuReportes_Click(object sender, EventArgs e)
        {
            AbrirHijo(new FrmReportes());
        }

        /// <summary>
        /// Abre el formulario como hijo MDI. Si ya estaba abierto lo trae al frente en
        /// lugar de duplicarlo: dos copias del mismo módulo con datos distintos son
        /// una fuente segura de confusión.
        /// </summary>
        private void AbrirHijo(Form hijo)
        {
            try
            {
                Form abierto = MdiChildren.FirstOrDefault(f => f.GetType() == hijo.GetType());
                if (abierto != null)
                {
                    hijo.Dispose();
                    abierto.Activate();
                    return;
                }

                hijo.MdiParent = this;
                hijo.WindowState = FormWindowState.Maximized;
                hijo.Show();
            }
            catch (Exception ex)
            {
                // Un módulo que no abre no debe tumbar la aplicación entera.
                Registro.Error("FrmPrincipal.AbrirHijo", ex);
                MessageBox.Show(
                    "No fue posible abrir el módulo solicitado.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -------------------------------------------------------------------
        // Sistema
        // -------------------------------------------------------------------

        private void MnuCerrarSesion_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea cerrar la sesión actual?", "Cerrar sesión",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            ReiniciarSesion = true;
            Close();
        }

        private void MnuSalir_Click(object sender, EventArgs e)
        {
            ReiniciarSesion = false;
            Close();
        }

        private void MnuAcerca_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                Tema.NombreHospitalCorto + "\n" +
                "Sistema de Atenciones Ambulatorias — versión 2.0\n\n" +
                "C# / .NET Framework 4.7.2 · Windows Forms · SQL Server · ReportViewer\n" +
                "Arquitectura por capas: Presentación, Negocio, Acceso a Datos, Integración.\n\n" +
                "Catálogo de diagnósticos: CIE-10 oficial del MINSA (español), almacenado localmente.",
                "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
