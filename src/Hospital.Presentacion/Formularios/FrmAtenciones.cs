using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hospital.Entidades;
using Hospital.Negocio;
using Hospital.Utilidades;

namespace Hospital.Presentacion.Formularios
{
    /// <summary>Consulta de atenciones con filtros y acciones del CRUD.</summary>
    public partial class FrmAtenciones : Form
    {
        private readonly AtencionServicio _servicio = new AtencionServicio();
        private readonly MaestroServicio _maestros = new MaestroServicio();
        private List<AtencionResumen> _resultados = new List<AtencionResumen>();

        public FrmAtenciones()
        {
            InitializeComponent();
            ConfigurarDisenoAdaptable();
            Load += FrmAtenciones_Load;
        }

        /// <summary>Completa el comportamiento adaptable ya definido para la grilla y los botones.</summary>
        private void ConfigurarDisenoAdaptable()
        {
            gbFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void FrmAtenciones_Load(object sender, EventArgs e)
        {
            CargarCombos();
            LimpiarFiltros();
            Buscar();
        }

        private void CargarCombos()
        {
            try
            {
                var medicos = new List<Medico> { new Medico { IdMedico = 0, Apellidos = "Todos", Nombres = "los médicos", Especialidad = "" } };
                medicos.AddRange(_maestros.ListarMedicos());
                cboMedico.DataSource = medicos;
                cboMedico.DisplayMember = "NombreCompleto";
                cboMedico.ValueMember = "IdMedico";

                cboEstado.Items.Clear();
                cboEstado.Items.Add(new ElementoLista("", "Todos los estados"));
                cboEstado.Items.Add(new ElementoLista("R", "Registrada"));
                cboEstado.Items.Add(new ElementoLista("A", "Atendida"));
                cboEstado.Items.Add(new ElementoLista("N", "Anulada"));
                cboEstado.DisplayMember = "Texto";
                cboEstado.ValueMember = "Valor";
                cboEstado.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ManejarError(ex);
            }
        }

        #region Consulta

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void Buscar()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                var filtro = new FiltroAtencion
                {
                    FechaDesde = chkFechas.Checked ? dtpDesde.Value.Date : (DateTime?)null,
                    FechaHasta = chkFechas.Checked ? dtpHasta.Value.Date : (DateTime?)null,
                    Busqueda   = string.IsNullOrWhiteSpace(txtBusqueda.Text) ? null : txtBusqueda.Text.Trim(),
                    IdMedico   = (cboMedico.SelectedValue != null && (int)cboMedico.SelectedValue > 0)
                                 ? (int)cboMedico.SelectedValue : (int?)null,
                    Estado     = ObtenerEstadoSeleccionado()
                };

                _resultados = _servicio.Listar(filtro);
                dgvAtenciones.DataSource = null;
                dgvAtenciones.DataSource = _resultados;

                lblResumen.Text = _resultados.Count == 0
                    ? "No hay atenciones que coincidan con los filtros."
                    : _resultados.Count + (_resultados.Count == 1 ? " atención encontrada." : " atenciones encontradas.");

                ActualizarBotones();
            }
            catch (Exception ex)
            {
                ManejarError(ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private string ObtenerEstadoSeleccionado()
        {
            var elemento = cboEstado.SelectedItem as ElementoLista;
            if (elemento == null || string.IsNullOrEmpty(elemento.Valor)) return null;
            return elemento.Valor;
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFiltros();
            Buscar();
        }

        private void LimpiarFiltros()
        {
            chkFechas.Checked = true;
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            txtBusqueda.Clear();
            if (cboMedico.Items.Count > 0) cboMedico.SelectedIndex = 0;
            if (cboEstado.Items.Count > 0) cboEstado.SelectedIndex = 0;
        }

        private void ChkFechas_CheckedChanged(object sender, EventArgs e)
        {
            dtpDesde.Enabled = chkFechas.Checked;
            dtpHasta.Enabled = chkFechas.Checked;
        }

        private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            Buscar();
        }

        #endregion

        #region Acciones del CRUD

        private void BtnNueva_Click(object sender, EventArgs e)
        {
            using (var formulario = new FrmAtencionEdicion())
            {
                if (formulario.ShowDialog(this) == DialogResult.OK) Buscar();
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            AtencionResumen seleccionada = ObtenerSeleccionada();
            if (seleccionada == null) return;

            if (seleccionada.Estado == "N")
            {
                MessageBox.Show("Una atención anulada solo puede consultarse, no modificarse.",
                                "Atención anulada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var formulario = new FrmAtencionEdicion(seleccionada.IdAtencion))
            {
                if (formulario.ShowDialog(this) == DialogResult.OK) Buscar();
            }
        }

        private void BtnAnular_Click(object sender, EventArgs e)
        {
            AtencionResumen seleccionada = ObtenerSeleccionada();
            if (seleccionada == null) return;

            string motivo = FrmMotivo.Solicitar(this, seleccionada.NumeroAtencion);
            if (motivo == null) return;

            try
            {
                _servicio.Anular(seleccionada.IdAtencion, motivo);
                MessageBox.Show("La atención " + seleccionada.NumeroAtencion + " quedó anulada.",
                                "Anulación registrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Buscar();
            }
            catch (Exception ex)
            {
                ManejarError(ex);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            AtencionResumen seleccionada = ObtenerSeleccionada();
            if (seleccionada == null) return;

            if (MessageBox.Show(
                    "Se eliminarán de forma permanente la atención " + seleccionada.NumeroAtencion +
                    " y sus " + seleccionada.TotalDiagnosticos + " diagnóstico(s).\n\n" +
                    "Esta acción no se puede deshacer. ¿Desea continuar?",
                    "Eliminar atención", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

            try
            {
                _servicio.Eliminar(seleccionada.IdAtencion);
                Buscar();
            }
            catch (Exception ex)
            {
                ManejarError(ex);
            }
        }

        private void DgvAtenciones_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            BtnEditar_Click(sender, EventArgs.Empty);
        }

        private AtencionResumen ObtenerSeleccionada()
        {
            if (dgvAtenciones.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una atención en la lista.", "Sin selección",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            return dgvAtenciones.CurrentRow.DataBoundItem as AtencionResumen;
        }

        private void ActualizarBotones()
        {
            bool hayDatos = _resultados.Count > 0;
            btnEditar.Enabled = hayDatos;
            btnAnular.Enabled = hayDatos;
            btnEliminar.Enabled = hayDatos && Sesion.EsAdministrador;
        }

        #endregion

        /// <summary>Colorea la columna de estado para leer la situación clínica de un vistazo.</summary>
        private void DgvAtenciones_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvAtenciones.Columns[e.ColumnIndex].Name != "colEstado") return;

            var fila = dgvAtenciones.Rows[e.RowIndex].DataBoundItem as AtencionResumen;
            if (fila == null) return;

            e.CellStyle.ForeColor = Tema.ColorEstado(fila.Estado);
            e.CellStyle.Font = Tema.FuenteEtiqueta;
        }

        private static void ManejarError(Exception ex)
        {
            if (ex is NegocioException)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Registro.Error("FrmAtenciones", ex);
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
