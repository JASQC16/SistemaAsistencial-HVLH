using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hospital.Entidades;
using Hospital.Negocio;
using Hospital.Utilidades;

namespace Hospital.Presentacion.Formularios
{
    /// <summary>
    /// Agenda de citas: programar, reprogramar y registrar el desenlace.
    ///
    /// El estado ATENDIDO no aparece entre las acciones disponibles a propósito. Lo
    /// asigna el registro de la atención, de modo que sea imposible declarar atendido
    /// a un paciente sin que exista el acto clínico correspondiente.
    /// </summary>
    public partial class FrmCitas : Form
    {
        private readonly CitaServicio _servicio = new CitaServicio();
        private readonly MaestroServicio _maestros = new MaestroServicio();
        private List<Cita> _citas = new List<Cita>();

        public FrmCitas()
        {
            InitializeComponent();
            ConfigurarDisenoAdaptable();
            ConfigurarColumnas();
        }

        /// <summary>
        /// Mantiene visibles la agenda y sus botones cuando el formulario MDI
        /// dispone de menos alto que el usado durante el diseño.
        /// </summary>
        private void ConfigurarDisenoAdaptable()
        {
            grpFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grdCitas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlAcciones.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblResumen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void FrmCitas_Load(object sender, EventArgs e)
        {
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today.AddDays(30);

            CargarCombos();
            Buscar();
        }

        // -------------------------------------------------------------------
        // Configuración
        // -------------------------------------------------------------------

        private void ConfigurarColumnas()
        {
            grdCitas.AutoGenerateColumns = false;
            grdCitas.Columns.Clear();

            var fecha = Columna("FechaCita", "Fecha y hora", 130);
            fecha.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

            grdCitas.Columns.AddRange(
                Columna("NumeroCita", "N.º cita", 110),
                fecha,
                Columna("DocumentoPaciente", "Documento", 95),
                Columna("Paciente", "Paciente", 230),
                Columna("EdadPaciente", "Edad", 50),
                Columna("Medico", "Profesional", 180),
                Columna("Especialidad", "Servicio", 130),
                Columna("EstadoDescripcion", "Estado", 100),
                Columna("NumeroAtencion", "N.º atención", 110),
                Columna("MotivoEstado", "Motivo del estado", 200));
        }

        private static DataGridViewTextBoxColumn Columna(string propiedad, string titulo, int ancho)
        {
            return new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo,
                Name = "col" + propiedad,
                Width = ancho,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        private void CargarCombos()
        {
            try
            {
                cboMedico.Items.Clear();
                cboMedico.Items.Add(new ElementoLista(null, "Todos los profesionales"));
                foreach (var medico in _maestros.ListarMedicos())
                {
                    cboMedico.Items.Add(new ElementoLista(
                        medico.IdMedico.ToString(), medico.NombreCompleto));
                }
                cboMedico.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Registro.Error("FrmCitas.CargarCombos", ex);
                Avisos.Error(this, "No fue posible cargar la lista de profesionales.");
            }

            cboEstado.Items.Clear();
            cboEstado.Items.Add(new ElementoLista(null, "Todos"));
            cboEstado.Items.Add(new ElementoLista(EstadoCita.Citado, "Citado"));
            cboEstado.Items.Add(new ElementoLista(EstadoCita.Atendido, "Atendido"));
            cboEstado.Items.Add(new ElementoLista(EstadoCita.NoAtendido, "No atendido"));
            cboEstado.Items.Add(new ElementoLista(EstadoCita.NoAcudio, "No acudió"));
            cboEstado.Items.Add(new ElementoLista(EstadoCita.Cancelado, "Cancelado"));
            cboEstado.SelectedIndex = 0;
        }

        // -------------------------------------------------------------------
        // Consulta
        // -------------------------------------------------------------------

        private void Buscar()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                var filtro = new FiltroCita
                {
                    FechaDesde = chkRango.Checked ? dtpDesde.Value.Date : (DateTime?)null,
                    FechaHasta = chkRango.Checked ? dtpHasta.Value.Date : (DateTime?)null,
                    Busqueda = string.IsNullOrWhiteSpace(txtBusqueda.Text) ? null : txtBusqueda.Text.Trim(),
                    IdMedico = EnteroSeleccionado(cboMedico),
                    Estado = ValorSeleccionado(cboEstado)
                };

                _citas = _servicio.Listar(filtro);

                grdCitas.DataSource = null;
                grdCitas.DataSource = _citas;

                lblResumen.Text = Resumir();
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

        /// <summary>Recuento por estado, que es la lectura que interesa de una agenda.</summary>
        private string Resumir()
        {
            if (_citas.Count == 0) return "No se encontraron citas con los criterios indicados.";

            int citado = 0, atendido = 0, noAtendido = 0, noAcudio = 0, cancelado = 0;
            foreach (var cita in _citas)
            {
                switch (cita.Estado)
                {
                    case EstadoCita.Citado:     citado++;     break;
                    case EstadoCita.Atendido:   atendido++;   break;
                    case EstadoCita.NoAtendido: noAtendido++; break;
                    case EstadoCita.NoAcudio:   noAcudio++;   break;
                    case EstadoCita.Cancelado:  cancelado++;  break;
                }
            }

            return string.Format(
                "{0} cita(s)   ·   citadas {1}   ·   atendidas {2}   ·   no atendidas {3}   ·   no acudieron {4}   ·   canceladas {5}",
                _citas.Count, citado, atendido, noAtendido, noAcudio, cancelado);
        }

        private Cita Seleccionada()
        {
            if (grdCitas.CurrentRow == null) return null;
            return grdCitas.CurrentRow.DataBoundItem as Cita;
        }

        private void ActualizarAcciones()
        {
            Cita cita = Seleccionada();
            bool modificable = cita != null && cita.EsModificable;

            btnEditar.Enabled = modificable;
            btnNoAtendido.Enabled = modificable;
            btnCancelar.Enabled = modificable;

            // La inasistencia solo tiene sentido una vez pasada la hora de la cita.
            btnNoAcudio.Enabled = modificable && cita.FechaCita <= DateTime.Now;
        }

        /// <summary>
        /// El color comunica el estado de la cita de un vistazo. Es la única
        /// información que la grilla transmite por color, para que siga leyéndose bien
        /// si alguien la imprime en blanco y negro: el texto del estado también está.
        /// </summary>
        private void GrdCitas_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= grdCitas.Rows.Count) return;

            var cita = grdCitas.Rows[e.RowIndex].DataBoundItem as Cita;
            if (cita == null) return;

            grdCitas.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Tema.ColorEstadoCita(cita.Estado);
        }

        // -------------------------------------------------------------------
        // Eventos de filtro
        // -------------------------------------------------------------------

        private void ChkRango_CheckedChanged(object sender, EventArgs e)
        {
            dtpDesde.Enabled = chkRango.Checked;
            dtpHasta.Enabled = chkRango.Checked;
            lblHasta.Enabled = chkRango.Checked;
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            chkRango.Checked = true;
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today.AddDays(30);
            txtBusqueda.Clear();
            cboMedico.SelectedIndex = 0;
            cboEstado.SelectedIndex = 0;
            Buscar();
        }

        private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            Buscar();
        }

        private void GrdCitas_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarAcciones();
        }

        private void GrdCitas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) Editar();
        }

        // -------------------------------------------------------------------
        // Acciones
        // -------------------------------------------------------------------

        private void BtnNueva_Click(object sender, EventArgs e)
        {
            using (var formulario = new FrmCitaEdicion())
            {
                if (formulario.ShowDialog(this) == DialogResult.OK) Buscar();
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            Editar();
        }

        private void Editar()
        {
            Cita cita = Seleccionada();
            if (cita == null) return;

            if (!cita.EsModificable)
            {
                Avisos.Advertencia(this,
                    "Solo puede reprogramarse una cita que siga en estado Citado.\n\n" +
                    "Estado actual: " + cita.EstadoDescripcion);
                return;
            }

            using (var formulario = new FrmCitaEdicion(cita.IdCita))
            {
                if (formulario.ShowDialog(this) == DialogResult.OK) Buscar();
            }
        }

        private void BtnNoAcudio_Click(object sender, EventArgs e)
        {
            CambiarEstado(EstadoCita.NoAcudio, "Registrar inasistencia",
                          "Motivo o constancia de la inasistencia del paciente");
        }

        private void BtnNoAtendido_Click(object sender, EventArgs e)
        {
            CambiarEstado(EstadoCita.NoAtendido, "Registrar no atendido",
                          "Motivo por el que el paciente acudió pero no fue atendido");
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            CambiarEstado(EstadoCita.Cancelado, "Cancelar cita",
                          "Motivo de la cancelación de la cita");
        }

        private void CambiarEstado(string nuevoEstado, string titulo, string indicacion)
        {
            Cita cita = Seleccionada();
            if (cita == null) return;

            string motivo = FrmTextoBreve.Solicitar(this, titulo,
                string.Format("Cita {0} — {1}", cita.NumeroCita, cita.Paciente), indicacion);

            if (motivo == null) return;   // el usuario canceló

            try
            {
                _servicio.CambiarEstado(cita, nuevoEstado, motivo);
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

        // -------------------------------------------------------------------
        // Apoyo
        // -------------------------------------------------------------------

        private static string ValorSeleccionado(ComboBox combo)
        {
            var elemento = combo.SelectedItem as ElementoLista;
            return elemento == null ? null : elemento.Valor;
        }

        private static int? EnteroSeleccionado(ComboBox combo)
        {
            string valor = ValorSeleccionado(combo);
            int numero;
            return int.TryParse(valor, out numero) ? numero : (int?)null;
        }
    }
}
