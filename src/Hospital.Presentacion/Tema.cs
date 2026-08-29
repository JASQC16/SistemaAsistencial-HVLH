using System.Drawing;
using System.Windows.Forms;

namespace Hospital.Presentacion
{
    /// <summary>
    /// Paleta y estilos de la aplicación en un solo lugar. Los formularios no definen
    /// colores por su cuenta: así la identidad visual es consistente y se puede cambiar
    /// desde un único archivo.
    ///
    /// Identidad institucional: Hospital Nacional Víctor Larco Herrera (HVLH).
    /// Colores oficiales: #209FE2 azul, #FEFEFE blanco, #E32619 rojo,
    /// #D6DBD5 gris claro y #FF9D42 naranja.
    ///
    /// Los tonos derivados (AzulOscuro, AzulSuave, NaranjaOscuro, Texto, TextoSuave)
    /// se obtienen de los oficiales aclarándolos u oscureciéndolos. Son necesarios para
    /// estados de hover, bordes y contraste de texto: usar solo los cinco colores planos
    /// dejaría textos ilegibles sobre fondo blanco.
    /// </summary>
    public static class Tema
    {
        // ---- Colores oficiales -------------------------------------------------
        public static readonly Color Azul     = ColorTranslator.FromHtml("#209FE2");
        public static readonly Color Blanco   = ColorTranslator.FromHtml("#FEFEFE");
        public static readonly Color Rojo     = ColorTranslator.FromHtml("#E32619");
        public static readonly Color Gris     = ColorTranslator.FromHtml("#D6DBD5");
        public static readonly Color Naranja  = ColorTranslator.FromHtml("#FF9D42");

        // ---- Tonos derivados ---------------------------------------------------
        public static readonly Color AzulOscuro    = ColorTranslator.FromHtml("#1A7DB4");
        public static readonly Color AzulProfundo  = ColorTranslator.FromHtml("#125C85");
        public static readonly Color AzulSuave     = ColorTranslator.FromHtml("#E6F4FC");
        public static readonly Color GrisSuave     = ColorTranslator.FromHtml("#F1F3F0");
        public static readonly Color GrisBorde     = ColorTranslator.FromHtml("#C3CAC1");
        public static readonly Color NaranjaOscuro = ColorTranslator.FromHtml("#C4701B");
        public static readonly Color RojoOscuro    = ColorTranslator.FromHtml("#B21D12");
        public static readonly Color Texto         = ColorTranslator.FromHtml("#2E3A3F");
        public static readonly Color TextoSuave    = ColorTranslator.FromHtml("#75837F");

        // Alias conservados para no romper código existente que los referenciaba.
        public static Color Fondo      { get { return Blanco; } }
        public static Color Seleccion  { get { return AzulSuave; } }

        // ---- Tipografía --------------------------------------------------------
        public static readonly Font FuenteTitulo     = new Font("Segoe UI Semibold", 15F, FontStyle.Regular);
        public static readonly Font FuenteTituloGran = new Font("Segoe UI Semibold", 17F, FontStyle.Regular);
        public static readonly Font FuenteSubtitulo  = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font FuenteBase       = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font FuenteEtiqueta   = new Font("Segoe UI Semibold", 9F, FontStyle.Regular);

        // ---- Identidad institucional ------------------------------------------
        public const string NombreHospital      = "Hospital Nacional Víctor Larco Herrera";
        public const string SiglaHospital       = "HVLH";
        public const string NombreHospitalCorto = "Hospital Nacional Víctor Larco Herrera - HVLH";

        public static void AplicarFormulario(Form formulario)
        {
            formulario.BackColor = Blanco;
            formulario.ForeColor = Texto;
            formulario.Font = FuenteBase;
            formulario.StartPosition = FormStartPosition.CenterScreen;
            formulario.Icon = Recursos.Icono;
        }

        /// <summary>Botón de acción principal: fondo azul institucional, texto blanco.</summary>
        public static void BotonPrimario(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Azul;
            boton.ForeColor = Blanco;
            boton.Font = FuenteEtiqueta;
            boton.Cursor = Cursors.Hand;
            boton.FlatAppearance.MouseOverBackColor = AzulOscuro;
            boton.UseVisualStyleBackColor = false;
        }

        /// <summary>Botón secundario: blanco con borde, para acciones no destructivas.</summary>
        public static void BotonSecundario(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 1;
            boton.FlatAppearance.BorderColor = GrisBorde;
            boton.BackColor = Blanco;
            boton.ForeColor = Texto;
            boton.Font = FuenteEtiqueta;
            boton.Cursor = Cursors.Hand;
            boton.FlatAppearance.MouseOverBackColor = GrisSuave;
            boton.UseVisualStyleBackColor = false;
        }

        /// <summary>Botón de acción destructiva (anular / eliminar).</summary>
        public static void BotonPeligro(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Rojo;
            boton.ForeColor = Blanco;
            boton.Font = FuenteEtiqueta;
            boton.Cursor = Cursors.Hand;
            boton.FlatAppearance.MouseOverBackColor = RojoOscuro;
            boton.UseVisualStyleBackColor = false;
        }

        /// <summary>Botón de acción que requiere atención pero no es destructiva.</summary>
        public static void BotonAtencion(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Naranja;
            boton.ForeColor = Color.White;
            boton.Font = FuenteEtiqueta;
            boton.Cursor = Cursors.Hand;
            boton.FlatAppearance.MouseOverBackColor = NaranjaOscuro;
            boton.UseVisualStyleBackColor = false;
        }

        public static void Grupo(GroupBox grupo)
        {
            grupo.BackColor = Blanco;
            grupo.ForeColor = AzulProfundo;
            grupo.Font = FuenteEtiqueta;
        }

        public static void Grilla(DataGridView grilla)
        {
            grilla.BackgroundColor = Blanco;
            grilla.BorderStyle = BorderStyle.FixedSingle;
            grilla.GridColor = Gris;
            grilla.EnableHeadersVisualStyles = false;
            grilla.RowHeadersVisible = false;
            grilla.AllowUserToAddRows = false;
            grilla.AllowUserToDeleteRows = false;
            grilla.AllowUserToResizeRows = false;
            grilla.MultiSelect = false;
            grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grilla.Font = FuenteBase;

            grilla.ColumnHeadersDefaultCellStyle.BackColor = Azul;
            grilla.ColumnHeadersDefaultCellStyle.ForeColor = Blanco;
            grilla.ColumnHeadersDefaultCellStyle.Font = FuenteEtiqueta;
            grilla.ColumnHeadersDefaultCellStyle.SelectionBackColor = Azul;
            grilla.ColumnHeadersDefaultCellStyle.SelectionForeColor = Blanco;
            grilla.ColumnHeadersHeight = 32;
            grilla.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            grilla.DefaultCellStyle.BackColor = Blanco;
            grilla.DefaultCellStyle.ForeColor = Texto;
            grilla.DefaultCellStyle.SelectionBackColor = AzulSuave;
            grilla.DefaultCellStyle.SelectionForeColor = Texto;
            grilla.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            grilla.AlternatingRowsDefaultCellStyle.BackColor = GrisSuave;
            grilla.RowTemplate.Height = 28;
        }

        public static void CampoTexto(Control campo)
        {
            campo.BackColor = Color.White;
            campo.ForeColor = Texto;
            campo.Font = FuenteBase;
        }

        /// <summary>Panel de encabezado blanco con franja azul inferior, para el logo institucional.</summary>
        public static void PanelEncabezado(Panel panel)
        {
            panel.BackColor = Blanco;
            panel.Paint += (remitente, argumentos) =>
            {
                var control = (Panel)remitente;
                using (var lapiz = new Pen(Azul, 3))
                {
                    argumentos.Graphics.DrawLine(lapiz, 0, control.Height - 2, control.Width, control.Height - 2);
                }
            };
        }

        /// <summary>Color asociado al estado clínico de la atención (R, A, N).</summary>
        public static Color ColorEstado(string estado)
        {
            switch (estado)
            {
                case "A": return Azul;
                case "N": return Rojo;
                default:  return NaranjaOscuro;
            }
        }

        /// <summary>Color asociado al estado de la cita.</summary>
        public static Color ColorEstadoCita(string estado)
        {
            switch (estado)
            {
                case "ATENDIDO":    return Azul;
                case "CITADO":      return NaranjaOscuro;
                case "NO_ATENDIDO": return RojoOscuro;
                case "NO_ACUDIO":   return Rojo;
                case "CANCELADO":   return TextoSuave;
                default:            return Texto;
            }
        }
    }
}
