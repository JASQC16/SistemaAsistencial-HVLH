using System.Drawing;
using System.Windows.Forms;

namespace Hospital.Presentacion.Controles
{
    /// <summary>
    /// Encabezado institucional reutilizable: logo del Hospital Nacional Víctor Larco
    /// Herrera, nombre de la institución y título del módulo.
    ///
    /// Existe como control propio y no copiado en cada formulario para que la
    /// identidad visual se defina una sola vez: si mañana cambia el logo o el nombre
    /// oficial, se toca un único archivo y no doce formularios.
    /// </summary>
    public class EncabezadoHvlh : Panel
    {
        private readonly PictureBox _logo;
        private readonly Label _institucion;
        private readonly Label _titulo;
        private readonly Label _subtitulo;

        public EncabezadoHvlh()
        {
            Height = 74;
            Dock = DockStyle.Top;
            BackColor = Tema.Blanco;
            Padding = new Padding(0);

            _logo = new PictureBox
            {
                Location = new Point(14, 8),
                Size = new Size(56, 56),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Image = Recursos.Logo
            };

            _institucion = new Label
            {
                AutoSize = true,
                Location = new Point(82, 12),
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Regular),
                ForeColor = Tema.AzulProfundo,
                Text = Tema.NombreHospitalCorto
            };

            _titulo = new Label
            {
                AutoSize = true,
                Location = new Point(84, 36),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Regular),
                ForeColor = Tema.Texto,
                Text = string.Empty
            };

            _subtitulo = new Label
            {
                AutoSize = true,
                Location = new Point(84, 55),
                Font = Tema.FuenteSubtitulo,
                ForeColor = Tema.TextoSuave,
                Text = string.Empty
            };

            Controls.Add(_logo);
            Controls.Add(_institucion);
            Controls.Add(_titulo);
            Controls.Add(_subtitulo);
        }

        /// <summary>Nombre del módulo, por ejemplo "Gestión de pacientes".</summary>
        public string Titulo
        {
            get { return _titulo.Text; }
            set { _titulo.Text = value; }
        }

        /// <summary>Línea de apoyo opcional bajo el título.</summary>
        public string Subtitulo
        {
            get { return _subtitulo.Text; }
            set { _subtitulo.Text = value; }
        }

        /// <summary>
        /// Franja azul institucional al pie del encabezado. Se dibuja en lugar de usar
        /// un panel adicional de 3 píxeles: es una línea, no una estructura.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var lapiz = new Pen(Tema.Azul, 3))
            {
                e.Graphics.DrawLine(lapiz, 0, Height - 2, Width, Height - 2);
            }
        }
    }
}
