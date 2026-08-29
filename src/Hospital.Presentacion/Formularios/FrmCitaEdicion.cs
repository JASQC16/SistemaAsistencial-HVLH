using System;
using System.Windows.Forms;
using Hospital.Entidades;
using Hospital.Negocio;
using Hospital.Utilidades;

namespace Hospital.Presentacion.Formularios
{
    /// <summary>
    /// Programación y reprogramación de una cita.
    ///
    /// El paciente se elige de los ya registrados; si no existe, se abre el módulo de
    /// pacientes para darlo de alta y se vuelve con él seleccionado. Nunca se crea un
    /// paciente "al vuelo" con datos incompletos: es justamente así como se terminan
    /// duplicando personas en un sistema asistencial.
    /// </summary>
    public partial class FrmCitaEdicion : Form
    {
        private readonly CitaServicio _servicio = new CitaServicio();
        private readonly MaestroServicio _maestros = new MaestroServicio();
        private readonly int _idCita;

        private Cita _cita;
        private Paciente _paciente;

        public FrmCitaEdicion() : this(0) { }

        public FrmCitaEdicion(int idCita)
        {
            InitializeComponent();
            _idCita = idCita;
        }

        private void FrmCitaEdicion_Load(object sender, EventArgs e)
        {
            CargarMedicos();

            // La agenda no se programa hacia atrás ni con más de un año de holgura.
            dtpFecha.MinDate = DateTime.Today.AddYears(-1);
            dtpFecha.MaxDate = DateTime.Today.AddYears(1);

            if (_idCita > 0) Cargar();
            else PrepararNueva();
        }

        private void CargarMedicos()
        {
            try
            {
                cboMedico.Items.Clear();
                cboMedico.Items.Add(new ElementoLista(null, "— Seleccione un profesional —"));
                foreach (var medico in _maestros.ListarMedicos())
                {
                    cboMedico.Items.Add(new ElementoLista(medico.IdMedico.ToString(), medico.NombreCompleto));
                }
                cboMedico.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Registro.Error("FrmCitaEdicion.CargarMedicos", ex);
                Avisos.Error(this, "No fue posible cargar la lista de profesionales.");
            }
        }

        private void PrepararNueva()
        {
            _cita = new Cita();
            encabezado.Titulo = "Nueva cita";
            encabezado.Subtitulo = "Consulta externa · horario de atención de 07:00 a 20:00, de lunes a sábado";
            Text = "HVLH — Nueva cita";

            DateTime propuesta = DateTime.Today.AddDays(1).AddHours(9);
            dtpFecha.Value = propuesta.Date;
            dtpHora.Value = propuesta;
        }

        private void Cargar()
        {
            try
            {
                _cita = _servicio.ObtenerPorId(_idCita);

                _paciente = new Paciente
                {
                    IdPaciente = _cita.IdPaciente,
                    TipoDocumento = _cita.TipoDocumento,
                    NumeroDocumento = _cita.DocumentoPaciente,
                    HistoriaClinica = _cita.HistoriaClinica
                };

                txtPaciente.Text = _cita.Paciente;
                lblDatosPaciente.Text = string.Format("{0} {1}   ·   historia clínica {2}   ·   {3} año(s)",
                    _cita.TipoDocumento, _cita.DocumentoPaciente, _cita.HistoriaClinica, _cita.EdadPaciente);

                Seleccionar(cboMedico, _cita.IdMedico.ToString());
                dtpFecha.Value = _cita.FechaCita.Date;
                dtpHora.Value = _cita.FechaCita;
                txtMotivo.Text = _cita.MotivoCita;
                txtObservaciones.Text = _cita.Observaciones;

                encabezado.Titulo = "Reprogramación de cita " + _cita.NumeroCita;
                Text = "HVLH — Cita " + _cita.NumeroCita;
                lblEstadoCita.Text = "Estado actual: " + _cita.EstadoDescripcion;

                // Una cita ya cerrada forma parte de la producción reportada: sus datos
                // se consultan, no se modifican.
                if (!_cita.EsModificable) BloquearEdicion();
            }
            catch (NegocioException ex)
            {
                Avisos.Advertencia(this, ex.Message);
                DialogResult = DialogResult.Cancel;
                Close();
            }
            catch (DatosException ex)
            {
                Avisos.Error(this, ex.Message);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void BloquearEdicion()
        {
            foreach (Control control in grpDatos.Controls)
            {
                var caja = control as TextBox;
                if (caja != null) { caja.ReadOnly = true; continue; }
                if (control is ComboBox || control is DateTimePicker || control is Button) control.Enabled = false;
            }

            btnGuardar.Visible = false;
            btnCancelar.Text = "Cerrar";
            lblEstadoCita.Text = "Estado actual: " + _cita.EstadoDescripcion +
                                 ". Solo una cita en estado Citado admite modificaciones.";
        }

        // -------------------------------------------------------------------
        // Selección de paciente
        // -------------------------------------------------------------------

        private void BtnBuscarPaciente_Click(object sender, EventArgs e)
        {
            using (var buscador = new FrmBuscarPaciente(txtPaciente.Text))
            {
                if (buscador.ShowDialog(this) != DialogResult.OK) return;
                AsignarPaciente(buscador.PacienteSeleccionado);
            }
        }

        private void BtnNuevoPaciente_Click(object sender, EventArgs e)
        {
            using (var formulario = new FrmPacienteEdicion())
            {
                if (formulario.ShowDialog(this) != DialogResult.OK) return;
                AsignarPaciente(formulario.PacienteGuardado);
            }
        }

        private void AsignarPaciente(Paciente paciente)
        {
            if (paciente == null) return;

            _paciente = paciente;
            txtPaciente.Text = paciente.NombreCompleto;
            lblDatosPaciente.Text = string.Format("{0} {1}   ·   historia clínica {2}   ·   {3} año(s)   ·   {4}",
                paciente.TipoDocumento, paciente.NumeroDocumento,
                paciente.HistoriaClinica, paciente.Edad, paciente.SexoDescripcion);
        }

        // -------------------------------------------------------------------
        // Guardado
        // -------------------------------------------------------------------

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            btnGuardar.Enabled = false;

            try
            {
                if (_paciente == null || _paciente.IdPaciente <= 0)
                {
                    Avisos.Advertencia(this, "Seleccione el paciente de la cita.");
                    return;
                }

                int? idMedico = EnteroSeleccionado(cboMedico);
                if (!idMedico.HasValue)
                {
                    Avisos.Advertencia(this, "Seleccione el profesional que atenderá la cita.");
                    return;
                }

                _cita.IdPaciente = _paciente.IdPaciente;
                _cita.IdMedico = idMedico.Value;
                _cita.FechaCita = dtpFecha.Value.Date
                    .AddHours(dtpHora.Value.Hour)
                    .AddMinutes(dtpHora.Value.Minute);
                _cita.MotivoCita = txtMotivo.Text;
                _cita.Observaciones = txtObservaciones.Text;
                _cita.IdUsuarioRegistro = Sesion.IdUsuario;

                if (_idCita > 0)
                {
                    _servicio.Actualizar(_cita);
                }
                else
                {
                    _servicio.Registrar(_cita);
                    Avisos.Informacion(this, "Cita registrada correctamente.\n\nNúmero: " + _cita.NumeroCita);
                }

                DialogResult = DialogResult.OK;
                Close();
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
                btnGuardar.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // -------------------------------------------------------------------
        // Apoyo
        // -------------------------------------------------------------------

        private static int? EnteroSeleccionado(ComboBox combo)
        {
            var elemento = combo.SelectedItem as ElementoLista;
            int numero;
            return elemento != null && int.TryParse(elemento.Valor, out numero) ? numero : (int?)null;
        }

        private static void Seleccionar(ComboBox combo, string valor)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                var elemento = combo.Items[i] as ElementoLista;
                if (elemento != null && elemento.Valor == valor)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }
    }
}
