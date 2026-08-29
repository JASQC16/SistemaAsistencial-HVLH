using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hospital.Entidades;
using Hospital.Negocio;
using Hospital.Utilidades;

namespace Hospital.Presentacion.Formularios
{
    /// <summary>
    /// Registro y edición de una atención con su detalle de diagnósticos.
    /// El formulario solo arma la entidad y muestra mensajes: las validaciones y el
    /// guardado transaccional viven en las capas de negocio y de datos.
    /// </summary>
    public partial class FrmAtencionEdicion : Form
    {
        private readonly AtencionServicio _servicio = new AtencionServicio();
        private readonly MaestroServicio _maestros = new MaestroServicio();
        private readonly Cie10Servicio _cie10 = new Cie10Servicio();
        private readonly CitaServicio _citas = new CitaServicio();

        private readonly int _idAtencion;
        private readonly BindingSource _origenDetalle = new BindingSource();
        private List<AtencionDetalle> _detalles = new List<AtencionDetalle>();
        private Paciente _paciente;

        /// <summary>Cita que la atención tenía asignada al abrirse, para poder reponerla al editar.</summary>
        private int? _idCitaOriginal;

        /// <summary>Permite cancelar una búsqueda en curso cuando el usuario lanza otra.</summary>
        private CancellationTokenSource _cancelacionBusqueda;

        private bool EsNuevo { get { return _idAtencion == 0; } }

        public FrmAtencionEdicion() : this(0) { }

        public FrmAtencionEdicion(int idAtencion)
        {
            _idAtencion = idAtencion;
            InitializeComponent();
            AplicarTemaCampos();
            AjustarTamanoAPantalla();
            Load += FrmAtencionEdicion_Load;
            FormClosed += (s, e) => CancelarBusquedaEnCurso();
        }

        /// <summary>
        /// Esta lógica se mantiene fuera de InitializeComponent para no romper el
        /// diseñador visual de Windows Forms.
        /// </summary>
        private void AplicarTemaCampos()
        {
            foreach (Control control in gbCabecera.Controls)
                if (control is TextBox) Tema.CampoTexto(control);

            foreach (Control control in gbDetalle.Controls)
                if (control is TextBox) Tema.CampoTexto(control);
        }

        /// <summary>
        /// El formulario de atención fue diseñado con 750 px de alto de cliente.
        /// En una pantalla 1366x768 eso puede superar el área de trabajo disponible.
        /// Solo cuando hace falta, reduce proporcionalmente el formulario completo
        /// para mantener visibles Guardar y Cancelar.
        /// </summary>
        private void AjustarTamanoAPantalla()
        {
            Rectangle area = Screen.PrimaryScreen.WorkingArea;
            const int margen = 24;

            float escalaX = (area.Width - margen) / (float)Width;
            float escalaY = (area.Height - margen) / (float)Height;
            float escala = Math.Min(1f, Math.Min(escalaX, escalaY));

            if (escala < 0.995f)
            {
                SuspendLayout();
                Scale(new SizeF(escala, escala));
                ResumeLayout(true);
            }
        }

        #region Carga inicial

        private void FrmAtencionEdicion_Load(object sender, EventArgs e)
        {
            try
            {
                CargarCombos();
                _origenDetalle.DataSource = _detalles;
                dgvDetalle.DataSource = _origenDetalle;

                if (EsNuevo)
                {
                    Text = "Nueva atención";
                    lblTitulo.Text = "Nueva atención ambulatoria";
                    dtpFecha.Value = DateTime.Now;
                    cboEstado.SelectedIndex = 0;
                }
                else
                {
                    CargarAtencion();
                }
            }
            catch (Exception ex)
            {
                ManejarError(ex);
                Close();
            }
        }

        private void CargarCombos()
        {
            cboMedico.DataSource = _maestros.ListarMedicos();
            cboMedico.DisplayMember = "NombreCompleto";
            cboMedico.ValueMember = "IdMedico";

            cboEstado.Items.Clear();
            cboEstado.Items.Add("Registrada");
            cboEstado.Items.Add("Atendida");
            cboEstado.SelectedIndex = 0;

            cboTipoDiagnostico.Items.Clear();
            cboTipoDiagnostico.Items.Add("Presuntivo");
            cboTipoDiagnostico.Items.Add("Definitivo");
            cboTipoDiagnostico.Items.Add("Repetitivo");
            cboTipoDiagnostico.SelectedIndex = 0;
        }

        private void CargarAtencion()
        {
            Atencion atencion = _servicio.ObtenerPorId(_idAtencion);

            Text = "Editar atención " + atencion.NumeroAtencion;
            lblTitulo.Text = "Atención " + atencion.NumeroAtencion;

            _paciente = new Paciente
            {
                IdPaciente = atencion.IdPaciente,
                NumeroDocumento = atencion.DocumentoPaciente
            };
            txtDocumento.Text = atencion.DocumentoPaciente;
            lblPaciente.Text = atencion.NombrePaciente;

            cboMedico.SelectedValue = atencion.IdMedico;
            dtpFecha.Value = atencion.FechaAtencion;
            txtMotivo.Text = atencion.MotivoConsulta;
            txtTemperatura.Text = FormatearDecimal(atencion.Temperatura);
            txtPresion.Text = atencion.PresionArterial;
            txtFrecuencia.Text = atencion.FrecuenciaCardiaca.HasValue
                ? atencion.FrecuenciaCardiaca.Value.ToString(CultureInfo.CurrentCulture) : string.Empty;
            txtPeso.Text = FormatearDecimal(atencion.Peso);
            txtTalla.Text = FormatearDecimal(atencion.Talla);
            txtObservaciones.Text = atencion.Observaciones;
            cboEstado.SelectedIndex = atencion.Estado == "A" ? 1 : 0;

            _idCitaOriginal = atencion.IdCita;
            CargarCitasDelPaciente(atencion.IdCita);

            _detalles = atencion.Detalles;
            _origenDetalle.DataSource = _detalles;
            _origenDetalle.ResetBindings(false);
        }

        private static string FormatearDecimal(decimal? valor)
        {
            return valor.HasValue ? valor.Value.ToString("0.##", CultureInfo.CurrentCulture) : string.Empty;
        }

        #endregion

        #region Paciente

        private void BtnBuscarPaciente_Click(object sender, EventArgs e)
        {
            using (var buscador = new FrmBuscarPaciente(txtDocumento.Text))
            {
                if (buscador.ShowDialog(this) != DialogResult.OK) return;

                _paciente = buscador.PacienteSeleccionado;
                txtDocumento.Text = _paciente.NumeroDocumento;
                lblPaciente.Text = _paciente.NombreCompleto + "   ·   " + _paciente.Edad + " años   ·   " +
                                   (_paciente.Sexo == "F" ? "Femenino" : "Masculino");

                // Al cambiar de paciente, la cita anterior deja de tener sentido.
                CargarCitasDelPaciente(null);
            }
        }

        /// <summary>
        /// Carga las citas del paciente que todavía pueden convertirse en atención.
        ///
        /// La lista siempre incluye la opción "Sin cita previa": el hospital atiende
        /// también por demanda espontánea, y obligar a elegir una cita llevaría al
        /// personal a inventarse citas ficticias para poder registrar la atención,
        /// que es justamente lo que arruinaría las estadísticas de inasistencia.
        /// </summary>
        private void CargarCitasDelPaciente(int? idCitaSeleccionada)
        {
            cboCita.Items.Clear();
            cboCita.Items.Add(new ElementoLista(null, "Sin cita previa (demanda espontánea)"));

            if (_paciente != null && _paciente.IdPaciente > 0)
            {
                try
                {
                    foreach (var cita in _citas.ListarPendientesPorPaciente(_paciente.IdPaciente, _idCitaOriginal))
                    {
                        cboCita.Items.Add(new ElementoLista(
                            cita.IdCita.ToString(CultureInfo.InvariantCulture),
                            string.Format("{0}  ·  {1:dd/MM/yyyy HH:mm}  ·  {2}",
                                          cita.NumeroCita, cita.FechaCita, cita.Especialidad)));
                    }
                }
                catch (Exception ex)
                {
                    // No poder leer la agenda no debe impedir registrar la atención.
                    Registro.Error("FrmAtencionEdicion.CargarCitasDelPaciente", ex);
                }
            }

            cboCita.SelectedIndex = 0;

            if (idCitaSeleccionada.HasValue)
            {
                string buscado = idCitaSeleccionada.Value.ToString(CultureInfo.InvariantCulture);
                for (int i = 0; i < cboCita.Items.Count; i++)
                {
                    var elemento = cboCita.Items[i] as ElementoLista;
                    if (elemento != null && elemento.Valor == buscado) { cboCita.SelectedIndex = i; break; }
                }
            }

            lblAyudaCita.Text = cboCita.Items.Count > 1
                ? "Al guardar, la cita seleccionada pasa a ATENDIDO."
                : "El paciente no tiene citas pendientes: se registrará como demanda espontánea.";
        }

        #endregion

        #region Detalle: búsqueda en el catálogo CIE-10 del MINSA

        /// <summary>
        /// Busca en el catálogo CIE-10 oficial del MINSA almacenado en SQL Server, en
        /// español. Acepta indistintamente un código ("F20", "F20.0") o un texto
        /// ("esquizofrenia"), con coincidencias parciales.
        ///
        /// La consulta va en segundo plano y cualquier búsqueda anterior se cancela,
        /// para que escribir rápido no encole peticiones ni congele la ventana.
        /// </summary>
        private async void BtnBuscarCie_Click(object sender, EventArgs e)
        {
            CancelarBusquedaEnCurso();
            _cancelacionBusqueda = new CancellationTokenSource();

            btnBuscarCie.Enabled = false;
            lblEstadoApi.ForeColor = Tema.TextoSuave;
            lblEstadoApi.Text = "Buscando en el catálogo CIE-10 (MINSA)...";
            cboResultadosCie.DataSource = null;

            try
            {
                List<DiagnosticoCie10> resultados =
                    await _cie10.BuscarAsync(txtBuscarCie.Text, _cancelacionBusqueda.Token);

                if (IsDisposed) return;

                if (resultados.Count == 0)
                {
                    lblEstadoApi.Text = "Sin coincidencias en el catálogo. Puede ingresar el código manualmente.";
                    return;
                }

                cboResultadosCie.DataSource = resultados;
                cboResultadosCie.DisplayMember = "Descripcion";
                cboResultadosCie.ValueMember = "Codigo";
                cboResultadosCie.SelectedIndex = 0;
                lblEstadoApi.ForeColor = Tema.Azul;
                lblEstadoApi.Text = resultados.Count + " diagnóstico(s) encontrado(s) en el catálogo MINSA.";
            }
            catch (NegocioException ex)
            {
                lblEstadoApi.ForeColor = Tema.Rojo;
                lblEstadoApi.Text = ex.Message;
            }
            catch (Exception ex)
            {
                ManejarError(ex);
            }
            finally
            {
                if (!IsDisposed) btnBuscarCie.Enabled = true;
            }
        }

        private void CancelarBusquedaEnCurso()
        {
            if (_cancelacionBusqueda == null) return;

            _cancelacionBusqueda.Cancel();
            _cancelacionBusqueda.Dispose();
            _cancelacionBusqueda = null;
        }

        private void TxtBuscarCie_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            BtnBuscarCie_Click(sender, EventArgs.Empty);
        }

        #endregion

        #region Detalle: agregar y quitar líneas

        private void BtnAgregarDetalle_Click(object sender, EventArgs e)
        {
            var seleccionado = cboResultadosCie.SelectedItem as DiagnosticoCie10;

            string codigo = seleccionado != null
                ? seleccionado.Codigo
                : txtCodigoManual.Text.Trim().ToUpperInvariant().Replace(".", string.Empty);
            string descripcion = seleccionado != null ? seleccionado.Descripcion : txtDescripcionManual.Text.Trim();
            string versionCatalogo = seleccionado != null ? seleccionado.VersionCatalogo : null;

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(descripcion))
            {
                MessageBox.Show(
                    "Busque un diagnóstico en el catálogo o complete el código y la descripción manualmente.",
                    "Diagnóstico incompleto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_detalles.Any(d => string.Equals(d.CodigoCie10, codigo, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("El diagnóstico " + codigo + " ya está en el detalle de esta atención.",
                                "Diagnóstico repetido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _detalles.Add(new AtencionDetalle
            {
                Item = _detalles.Count + 1,
                CodigoCie10 = codigo,
                DescripcionDiagnostico = descripcion.Length > 250 ? descripcion.Substring(0, 250) : descripcion,
                TipoDiagnostico = ObtenerTipoDiagnostico(),
                Indicaciones = string.IsNullOrWhiteSpace(txtIndicaciones.Text) ? null : txtIndicaciones.Text.Trim(),
                VersionCatalogoCie10 = versionCatalogo
            });

            RefrescarDetalle();

            txtIndicaciones.Clear();
            txtCodigoManual.Clear();
            txtDescripcionManual.Clear();
        }

        private string ObtenerTipoDiagnostico()
        {
            switch (cboTipoDiagnostico.SelectedIndex)
            {
                case 1:  return "D";
                case 2:  return "R";
                default: return "P";
            }
        }

        private void BtnQuitarDetalle_Click(object sender, EventArgs e)
        {
            if (dgvDetalle.CurrentRow == null)
            {
                MessageBox.Show("Seleccione el diagnóstico que desea quitar.", "Sin selección",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var detalle = dgvDetalle.CurrentRow.DataBoundItem as AtencionDetalle;
            if (detalle == null) return;

            _detalles.Remove(detalle);
            RenumerarItems();
            RefrescarDetalle();
        }

        private void RenumerarItems()
        {
            for (int i = 0; i < _detalles.Count; i++) _detalles[i].Item = i + 1;
        }

        private void RefrescarDetalle()
        {
            _origenDetalle.DataSource = null;
            _origenDetalle.DataSource = _detalles;
            lblTotalDetalle.Text = _detalles.Count == 0
                ? "Sin diagnósticos registrados."
                : _detalles.Count + " diagnóstico(s) en el detalle.";
        }

        #endregion

        #region Guardar

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Atencion atencion;
            try
            {
                atencion = ArmarEntidad();
            }
            catch (NegocioException ex)
            {
                MessageBox.Show(ex.Message, "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor = Cursors.WaitCursor;
            btnGuardar.Enabled = false;
            try
            {
                if (EsNuevo)
                {
                    _servicio.Registrar(atencion);
                    MessageBox.Show("Se registró la atención " + atencion.NumeroAtencion + ".",
                                    "Atención guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _servicio.Actualizar(atencion);
                    MessageBox.Show("Se actualizó la atención.", "Cambios guardados",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ManejarError(ex);
            }
            finally
            {
                btnGuardar.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Traduce los controles a la entidad. Aquí solo se resuelve el formato de los
        /// datos; las reglas clínicas las evalúa AtencionServicio.
        /// </summary>
        /// <summary>Identificador de la cita elegida, o null si la atención no proviene de una.</summary>
        private int? CitaSeleccionada()
        {
            var elemento = cboCita.SelectedItem as ElementoLista;
            if (elemento == null || string.IsNullOrEmpty(elemento.Valor)) return null;

            int id;
            return int.TryParse(elemento.Valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out id)
                   ? id : (int?)null;
        }

        private Atencion ArmarEntidad()
        {
            if (_paciente == null || _paciente.IdPaciente <= 0)
                throw new NegocioException("Seleccione un paciente con el botón de búsqueda.");

            return new Atencion
            {
                IdAtencion         = _idAtencion,
                IdPaciente         = _paciente.IdPaciente,
                IdMedico           = cboMedico.SelectedValue != null ? (int)cboMedico.SelectedValue : 0,
                IdCita             = CitaSeleccionada(),
                FechaAtencion      = dtpFecha.Value,
                MotivoConsulta     = txtMotivo.Text.Trim(),
                Temperatura        = LeerDecimal(txtTemperatura.Text, "temperatura"),
                PresionArterial    = string.IsNullOrWhiteSpace(txtPresion.Text) ? null : txtPresion.Text.Trim(),
                FrecuenciaCardiaca = LeerEntero(txtFrecuencia.Text, "frecuencia cardiaca"),
                Peso               = LeerDecimal(txtPeso.Text, "peso"),
                Talla              = LeerDecimal(txtTalla.Text, "talla"),
                Observaciones      = string.IsNullOrWhiteSpace(txtObservaciones.Text) ? null : txtObservaciones.Text.Trim(),
                Estado             = cboEstado.SelectedIndex == 1 ? "A" : "R",
                Detalles           = _detalles
            };
        }

        private static decimal? LeerDecimal(string texto, string campo)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;

            string entrada = texto.Trim();
            decimal valor;

            // Se acepta el separador decimal de la configuración regional del equipo
            // y también el punto, que es lo que suele escribirse al digitar rápido.
            if (decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.CurrentCulture, out valor) ||
                decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.InvariantCulture, out valor) ||
                decimal.TryParse(entrada.Replace('.', ',').Replace(',', 
                    CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]),
                    NumberStyles.Number, CultureInfo.CurrentCulture, out valor))
            {
                return valor;
            }

            throw new NegocioException("El valor de " + campo + " no es un número válido.");
        }

        private static int? LeerEntero(string texto, string campo)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;

            int valor;
            if (!int.TryParse(texto.Trim(), out valor))
                throw new NegocioException("El valor de " + campo + " debe ser un número entero.");
            return valor;
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            if (_detalles.Count > 0 || !string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                if (MessageBox.Show("Se perderán los datos no guardados. ¿Desea salir?", "Cancelar registro",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                    MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            }

            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        private static void ManejarError(Exception ex)
        {
            if (ex is NegocioException)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Registro.Error("FrmAtencionEdicion", ex);
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
