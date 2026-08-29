using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hospital.Entidades;
using Hospital.Negocio;
using Hospital.Utilidades;

namespace Hospital.Presentacion.Formularios
{
    /// <summary>
    /// Módulo de pacientes: listar, buscar, registrar, editar, consultar y activar o
    /// desactivar.
    ///
    /// No existe eliminación física. Un paciente concentra su historia clínica, sus
    /// citas y sus atenciones; borrarlo dejaría todo eso huérfano. Lo que sí puede
    /// hacerse es desactivarlo, y ni siquiera eso mientras tenga citas pendientes.
    /// </summary>
    public partial class FrmPacientes : Form
    {
        private readonly PacienteServicio _servicio = new PacienteServicio();
        private List<Paciente> _pacientes = new List<Paciente>();

        public FrmPacientes()
        {
            InitializeComponent();
            ConfigurarDisenoAdaptable();
            ConfigurarColumnas();
        }

        /// <summary>
        /// Hace que el módulo se adapte al alto/ancho disponible dentro del MDI.
        /// El formulario fue diseñado a 1140x614, pero en pantallas de 1366x768
        /// el encabezado y las barras del formulario principal reducen el alto útil.
        /// Por eso la grilla se redimensiona y el panel de acciones permanece visible.
        /// </summary>
        private void ConfigurarDisenoAdaptable()
        {
            grpFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grdPacientes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlAcciones.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblResumen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void FrmPacientes_Load(object sender, EventArgs e)
        {
            CargarCombos();
            Buscar();
        }

        // -------------------------------------------------------------------
        // Configuración
        // -------------------------------------------------------------------

        /// <summary>
        /// Las columnas se declaran explícitamente en lugar de dejar que el grid las
        /// genere: así se controla el orden, el ancho y el formato, y añadir una
        /// propiedad a la entidad no cambia la pantalla sin querer.
        /// </summary>
        private void ConfigurarColumnas()
        {
            grdPacientes.AutoGenerateColumns = false;
            grdPacientes.Columns.Clear();
            grdPacientes.Columns.AddRange(
                Columna("HistoriaClinica", "H. clínica", 90),
                Columna("TipoDocumento", "Tipo", 55),
                Columna("NumeroDocumento", "Documento", 95),
                Columna("NombreCompleto", "Paciente", 260),
                Columna("Edad", "Edad", 50),
                Columna("SexoDescripcion", "Sexo", 75),
                Columna("Telefono", "Teléfono", 95),
                Columna("TotalCitas", "Citas", 55),
                Columna("TotalAtenciones", "Atenciones", 75),
                Columna("EstadoDescripcion", "Estado", 75));

            var fechaRegistro = Columna("FechaRegistro", "Registrado", 100);
            fechaRegistro.DefaultCellStyle.Format = "dd/MM/yyyy";
            grdPacientes.Columns.Add(fechaRegistro);
        }

        private static DataGridViewTextBoxColumn Columna(string propiedad, string titulo, int ancho)
        {
            return new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo,
                Name = "col" + propiedad,
                Width = ancho,
                // Un List<T> enlazado no es ordenable: dejar el encabezado clicable
                // provocaría una excepción al intentar ordenar.
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        private void CargarCombos()
        {
            cboTipoDocumento.Items.Clear();
            cboTipoDocumento.Items.Add(new ElementoLista(null, "Todos"));
            cboTipoDocumento.Items.Add(new ElementoLista("DNI", "DNI"));
            cboTipoDocumento.Items.Add(new ElementoLista("CE", "Carné de extranjería"));
            cboTipoDocumento.Items.Add(new ElementoLista("PAS", "Pasaporte"));
            cboTipoDocumento.Items.Add(new ElementoLista("CNV", "Certificado de nacido vivo"));
            cboTipoDocumento.SelectedIndex = 0;

            cboEstado.Items.Clear();
            cboEstado.Items.Add(new ElementoLista(null, "Todos"));
            cboEstado.Items.Add(new ElementoLista("1", "Activos"));
            cboEstado.Items.Add(new ElementoLista("0", "Inactivos"));
            cboEstado.SelectedIndex = 1;   // por defecto se trabaja con pacientes activos
        }

        // -------------------------------------------------------------------
        // Consulta
        // -------------------------------------------------------------------

        private void Buscar()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                string tipoDocumento = ValorSeleccionado(cboTipoDocumento);
                string textoEstado = ValorSeleccionado(cboEstado);
                bool? activo = textoEstado == null ? (bool?)null : textoEstado == "1";

                _pacientes = _servicio.Listar(txtBusqueda.Text, tipoDocumento, activo);

                grdPacientes.DataSource = null;
                grdPacientes.DataSource = _pacientes;

                lblResumen.Text = _pacientes.Count == 0
                    ? "No se encontraron pacientes con los criterios indicados."
                    : string.Format("{0} paciente(s) encontrado(s).", _pacientes.Count);

                ActualizarAcciones();
            }
            catch (NegocioException ex)
            {
                Avisos.Advertencia(this, ex.Message);
            }
            catch (DatosException ex)
            {
                Avisos.Error(this, ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private Paciente Seleccionado()
        {
            if (grdPacientes.CurrentRow == null) return null;
            return grdPacientes.CurrentRow.DataBoundItem as Paciente;
        }

        /// <summary>
        /// Los botones reflejan lo que realmente puede hacerse con la fila
        /// seleccionada. Es preferible a dejarlos siempre activos y responder con un
        /// mensaje de error después del clic.
        /// </summary>
        private void ActualizarAcciones()
        {
            Paciente paciente = Seleccionado();
            bool hay = paciente != null;

            btnEditar.Enabled = hay;
            btnConsultar.Enabled = hay;
            btnEstado.Enabled = hay;

            if (hay)
            {
                btnEstado.Text = paciente.Activo ? "Desactivar" : "Reactivar";
                if (paciente.Activo) Tema.BotonAtencion(btnEstado);
                else Tema.BotonPrimario(btnEstado);
            }
            else
            {
                btnEstado.Text = "Desactivar";
            }
        }

        // -------------------------------------------------------------------
        // Eventos
        // -------------------------------------------------------------------

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            txtBusqueda.Clear();
            cboTipoDocumento.SelectedIndex = 0;
            cboEstado.SelectedIndex = 1;
            Buscar();
        }

        private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            Buscar();
        }

        private void GrdPacientes_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarAcciones();
        }

        private void GrdPacientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) Editar();
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            using (var formulario = new FrmPacienteEdicion())
            {
                if (formulario.ShowDialog(this) != DialogResult.OK) return;

                Buscar();
                Avisos.Informacion(this,
                    "Paciente registrado correctamente.\n\nHistoria clínica: " + formulario.HistoriaClinicaGenerada);
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            Editar();
        }

        private void Editar()
        {
            Paciente paciente = Seleccionado();
            if (paciente == null) return;

            if (!paciente.Activo)
            {
                Avisos.Advertencia(this,
                    "El paciente está inactivo. Reactívelo antes de modificar sus datos.");
                return;
            }

            using (var formulario = new FrmPacienteEdicion(paciente.IdPaciente))
            {
                if (formulario.ShowDialog(this) == DialogResult.OK) Buscar();
            }
        }

        private void BtnConsultar_Click(object sender, EventArgs e)
        {
            Paciente paciente = Seleccionado();
            if (paciente == null) return;

            using (var formulario = new FrmPacienteEdicion(paciente.IdPaciente, soloLectura: true))
            {
                formulario.ShowDialog(this);
            }
        }

        private void BtnEstado_Click(object sender, EventArgs e)
        {
            Paciente paciente = Seleccionado();
            if (paciente == null) return;

            bool activar = !paciente.Activo;
            string accion = activar ? "reactivar" : "desactivar";

            string pregunta = string.Format("¿Desea {0} al paciente {1}?", accion, paciente.NombreCompleto);
            if (!activar)
            {
                pregunta += "\n\nNo se elimina ningún dato: su historia clínica, sus citas y sus " +
                            "atenciones se conservan. Simplemente dejará de aparecer al registrar " +
                            "nuevas citas y atenciones.";
            }

            if (!Avisos.Confirmar(this, pregunta, activar ? "Reactivar paciente" : "Desactivar paciente")) return;

            try
            {
                _servicio.CambiarEstado(paciente.IdPaciente, activar);
                Buscar();
            }
            catch (NegocioException ex)
            {
                Avisos.Advertencia(this, ex.Message);
            }
            catch (DatosException ex)
            {
                Avisos.Error(this, ex.Message);
            }
        }

        private static string ValorSeleccionado(ComboBox combo)
        {
            var elemento = combo.SelectedItem as ElementoLista;
            return elemento == null ? null : elemento.Valor;
        }
    }
}
