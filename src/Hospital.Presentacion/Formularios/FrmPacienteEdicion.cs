using System;
using System.Windows.Forms;
using Hospital.Entidades;
using Hospital.Negocio;
using Hospital.Utilidades;

namespace Hospital.Presentacion.Formularios
{
    /// <summary>
    /// Alta, edición y consulta de un paciente.
    ///
    /// El mismo formulario sirve para los tres casos, distinguidos por el
    /// constructor. Duplicarlo en tres pantallas casi idénticas garantizaría que las
    /// validaciones se desincronicen con el tiempo.
    /// </summary>
    public partial class FrmPacienteEdicion : Form
    {
        private readonly PacienteServicio _servicio = new PacienteServicio();
        private readonly int _idPaciente;
        private readonly bool _soloLectura;
        private Paciente _paciente;

        /// <summary>Historia clínica asignada por la base al registrar, para poder informarla.</summary>
        public string HistoriaClinicaGenerada { get; private set; }

        /// <summary>Paciente resultante, útil cuando el formulario se abre desde otro módulo.</summary>
        public Paciente PacienteGuardado { get; private set; }

        public FrmPacienteEdicion() : this(0, false) { }

        public FrmPacienteEdicion(int idPaciente) : this(idPaciente, false) { }

        public FrmPacienteEdicion(int idPaciente, bool soloLectura)
        {
            InitializeComponent();
            _idPaciente = idPaciente;
            _soloLectura = soloLectura;
        }

        private void FrmPacienteEdicion_Load(object sender, EventArgs e)
        {
            CargarCombos();

            // El calendario se limita al rango biológicamente posible: es más rápido
            // que validar después y evita errores de tipeo en el año.
            dtpFechaNacimiento.MinDate = DateTime.Today.AddYears(-120);
            dtpFechaNacimiento.MaxDate = DateTime.Today;

            if (_idPaciente > 0) Cargar();
            else PrepararNuevo();

            if (_soloLectura) AplicarSoloLectura();
        }

        private void CargarCombos()
        {
            cboTipoDocumento.Items.Clear();
            cboTipoDocumento.Items.Add(new ElementoLista("DNI", "DNI"));
            cboTipoDocumento.Items.Add(new ElementoLista("CE", "Carné de extranjería"));
            cboTipoDocumento.Items.Add(new ElementoLista("PAS", "Pasaporte"));
            cboTipoDocumento.Items.Add(new ElementoLista("CNV", "Certificado de nacido vivo"));
            cboTipoDocumento.Items.Add(new ElementoLista("OTR", "Otro"));
            cboTipoDocumento.SelectedIndex = 0;

            cboSexo.Items.Clear();
            cboSexo.Items.Add(new ElementoLista("M", "Masculino"));
            cboSexo.Items.Add(new ElementoLista("F", "Femenino"));
            cboSexo.SelectedIndex = 0;
        }

        private void PrepararNuevo()
        {
            _paciente = new Paciente();
            encabezado.Titulo = "Nuevo paciente";
            encabezado.Subtitulo = "Verifique que el paciente no esté ya registrado antes de crearlo";
            Text = "HVLH — Nuevo paciente";

            dtpFechaNacimiento.Value = DateTime.Today.AddYears(-30);
            txtHistoriaClinica.Text = "(se genera al guardar)";
            ActualizarEdad();
            txtNumeroDocumento.Focus();
        }

        private void Cargar()
        {
            try
            {
                _paciente = _servicio.ObtenerPorId(_idPaciente);

                Seleccionar(cboTipoDocumento, _paciente.TipoDocumento);
                txtNumeroDocumento.Text = _paciente.NumeroDocumento;
                txtHistoriaClinica.Text = _paciente.HistoriaClinica;
                txtApellidoPaterno.Text = _paciente.ApellidoPaterno;
                txtApellidoMaterno.Text = _paciente.ApellidoMaterno;
                txtNombres.Text = _paciente.Nombres;
                dtpFechaNacimiento.Value = _paciente.FechaNacimiento < dtpFechaNacimiento.MinDate
                    ? dtpFechaNacimiento.MinDate
                    : _paciente.FechaNacimiento;
                Seleccionar(cboSexo, _paciente.Sexo);
                txtTelefono.Text = _paciente.Telefono;
                txtCorreo.Text = _paciente.Correo;
                txtDireccion.Text = _paciente.Direccion;

                encabezado.Titulo = _soloLectura ? "Consulta de paciente" : "Edición de paciente";
                encabezado.Subtitulo = _paciente.NombreCompleto + "  ·  historia clínica " + _paciente.HistoriaClinica;
                Text = "HVLH — " + _paciente.NombreCompleto;

                lblEstadoRegistro.Text = string.Format(
                    "Estado: {0}   ·   registrado el {1:dd/MM/yyyy}{2}",
                    _paciente.EstadoDescripcion,
                    _paciente.FechaRegistro,
                    _paciente.FechaModificacion.HasValue
                        ? string.Format("   ·   última modificación {0:dd/MM/yyyy}", _paciente.FechaModificacion.Value)
                        : string.Empty);

                ActualizarEdad();
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

        /// <summary>Modo consulta: los datos se ven, no se tocan.</summary>
        private void AplicarSoloLectura()
        {
            foreach (Control grupo in new Control[] { grpIdentificacion, grpContacto })
            {
                foreach (Control control in grupo.Controls)
                {
                    var caja = control as TextBox;
                    if (caja != null) { caja.ReadOnly = true; continue; }

                    if (control is ComboBox || control is DateTimePicker) control.Enabled = false;
                }
            }

            btnGuardar.Visible = false;
            btnCancelar.Text = "Cerrar";
        }

        // -------------------------------------------------------------------
        // Eventos
        // -------------------------------------------------------------------

        private void DtpFechaNacimiento_ValueChanged(object sender, EventArgs e)
        {
            ActualizarEdad();
        }

        /// <summary>
        /// La edad se muestra calculada y no se almacena: una edad guardada en la base
        /// queda desactualizada al día siguiente del cumpleaños.
        /// </summary>
        private void ActualizarEdad()
        {
            DateTime nacimiento = dtpFechaNacimiento.Value.Date;
            int edad = DateTime.Today.Year - nacimiento.Year;
            if (nacimiento > DateTime.Today.AddYears(-edad)) edad--;
            txtEdad.Text = edad < 0 ? string.Empty : edad + " año(s)";
        }

        private void CboTipoDocumento_SelectedIndexChanged(object sender, EventArgs e)
        {
            // El DNI peruano tiene ocho dígitos exactos; los demás documentos admiten
            // formatos alfanuméricos de longitud variable.
            bool esDni = ValorDe(cboTipoDocumento) == "DNI";
            txtNumeroDocumento.MaxLength = esDni ? 8 : 15;
        }

        /// <summary>
        /// Aviso temprano de duplicado. Al salir del campo se consulta si ese documento
        /// ya existe, para que el usuario no llene el formulario entero y descubra el
        /// choque recién al guardar.
        /// </summary>
        private void TxtNumeroDocumento_Leave(object sender, EventArgs e)
        {
            if (_soloLectura) return;
            if (string.IsNullOrWhiteSpace(txtNumeroDocumento.Text)) return;

            try
            {
                var pacientes = _servicio.Buscar(txtNumeroDocumento.Text.Trim());
                foreach (var existente in pacientes)
                {
                    bool mismoDocumento =
                        string.Equals(existente.NumeroDocumento, txtNumeroDocumento.Text.Trim(),
                                      StringComparison.OrdinalIgnoreCase) &&
                        existente.TipoDocumento == ValorDe(cboTipoDocumento);

                    if (!mismoDocumento || existente.IdPaciente == _idPaciente) continue;

                    Avisos.Advertencia(this, string.Format(
                        "Ese documento ya corresponde a {0} (historia clínica {1}).\n\n" +
                        "No registre un paciente nuevo: use el existente para su cita o atención.",
                        existente.NombreCompleto, existente.HistoriaClinica));
                    return;
                }
            }
            catch (Exception ex)
            {
                // La verificación es una comodidad; si falla, la validación real al
                // guardar y la restricción UNIQUE siguen protegiendo los datos.
                Registro.Error("FrmPacienteEdicion.TxtNumeroDocumento_Leave", ex);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            btnGuardar.Enabled = false;

            try
            {
                Recoger();

                if (_idPaciente > 0)
                {
                    _servicio.Actualizar(_paciente);
                }
                else
                {
                    _servicio.Registrar(_paciente);
                    HistoriaClinicaGenerada = _paciente.HistoriaClinica;
                }

                PacienteGuardado = _paciente;
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

        private void Recoger()
        {
            _paciente.TipoDocumento   = ValorDe(cboTipoDocumento);
            _paciente.NumeroDocumento = txtNumeroDocumento.Text;
            _paciente.ApellidoPaterno = txtApellidoPaterno.Text;
            _paciente.ApellidoMaterno = txtApellidoMaterno.Text;
            _paciente.Nombres         = txtNombres.Text;
            _paciente.FechaNacimiento = dtpFechaNacimiento.Value.Date;
            _paciente.Sexo            = ValorDe(cboSexo);
            _paciente.Telefono        = txtTelefono.Text;
            _paciente.Correo          = txtCorreo.Text;
            _paciente.Direccion       = txtDireccion.Text;
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // -------------------------------------------------------------------
        // Apoyo
        // -------------------------------------------------------------------

        private static string ValorDe(ComboBox combo)
        {
            var elemento = combo.SelectedItem as ElementoLista;
            return elemento == null ? null : elemento.Valor;
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
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
        }
    }
}
