using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Hospital.Entidades;
using Hospital.Negocio;
using Hospital.Utilidades;
using Microsoft.Reporting.WinForms;

namespace Hospital.Presentacion.Formularios
{
    /// <summary>
    /// Reporte de pacientes y atenciones por rango de fechas, con filtros por estado,
    /// profesional, servicio, documento y diagnóstico CIE-10.
    ///
    /// Se renderiza con ReportViewer en modo local (RDLC), de modo que la impresión y
    /// la exportación a PDF, Excel y Word las resuelve el propio visor. Lo exportado
    /// coincide siempre con lo que se ve en pantalla porque ambos consumen el mismo
    /// DataTable: no hay una consulta para mostrar y otra para exportar.
    /// </summary>
    public partial class FrmReportes : Form
    {
        private const string NombreConjuntoDatos = "dsReporte";
        private const string RecursoIncrustado = "Hospital.Presentacion.Reportes.rptGeneralHVLH.rdlc";
        private const string ArchivoReporte = "rptGeneralHVLH.rdlc";

        private readonly ReporteServicio _servicio = new ReporteServicio();
        private readonly MaestroServicio _maestros = new MaestroServicio();

        /// <summary>Evita que el ajuste automático de fechas dispare recargas en cascada.</summary>
        private bool _cargando;

        public FrmReportes()
        {
            InitializeComponent();
        }

        // ===================================================================
        // Carga inicial
        // ===================================================================

        private void FrmReportes_Load(object sender, EventArgs e)
        {
            _cargando = true;
            try
            {
                CargarPeriodos();
                CargarEstados();
                CargarMedicos();
                CargarEspecialidades();

                dtpDesde.Value = DateTime.Today.AddDays(-30);
                dtpHasta.Value = DateTime.Today;

                visor.SetDisplayMode(DisplayMode.PrintLayout);
                visor.ZoomMode = ZoomMode.PageWidth;

                _cargando = false;
                Generar();
            }
            catch (Exception ex)
            {
                _cargando = false;
                ManejarError(ex);
            }
        }

        /// <summary>
        /// Atajos de periodo. El rango sigue siendo el criterio real: estos valores
        /// solo rellenan las fechas, y el usuario puede ajustarlas después.
        /// </summary>
        private void CargarPeriodos()
        {
            cboPeriodo.Items.Clear();
            cboPeriodo.Items.Add(new ElementoLista("HOY", "Hoy"));
            cboPeriodo.Items.Add(new ElementoLista("AYER", "Ayer"));
            cboPeriodo.Items.Add(new ElementoLista("SEMANA", "Últimos 7 días"));
            cboPeriodo.Items.Add(new ElementoLista("QUINCENA", "Últimos 15 días"));
            cboPeriodo.Items.Add(new ElementoLista("MES", "Últimos 30 días"));
            cboPeriodo.Items.Add(new ElementoLista("MES_ACTUAL", "Mes actual"));
            cboPeriodo.Items.Add(new ElementoLista("MES_ANTERIOR", "Mes anterior"));
            cboPeriodo.Items.Add(new ElementoLista("TRIMESTRE", "Últimos 3 meses"));
            cboPeriodo.Items.Add(new ElementoLista("ANIO", "Año actual"));
            cboPeriodo.Items.Add(new ElementoLista("PERSONALIZADO", "Personalizado"));
            cboPeriodo.SelectedIndex = 4;   // últimos 30 días
        }

        private void CargarEstados()
        {
            cboEstado.Items.Clear();
            cboEstado.Items.Add(new ElementoLista(null, "Todos"));
            cboEstado.Items.Add(new ElementoLista(EstadoCita.Atendido, "Atendidos"));
            cboEstado.Items.Add(new ElementoLista(EstadoCita.NoAtendido, "No atendidos"));
            cboEstado.Items.Add(new ElementoLista(EstadoCita.Citado, "Citados"));
            cboEstado.Items.Add(new ElementoLista(EstadoCita.NoAcudio, "No acudieron"));
            cboEstado.Items.Add(new ElementoLista(EstadoCita.Cancelado, "Cancelados"));
            cboEstado.SelectedIndex = 0;
        }

        private void CargarMedicos()
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

        private void CargarEspecialidades()
        {
            cboEspecialidad.Items.Clear();
            cboEspecialidad.Items.Add(new ElementoLista(null, "Todos los servicios"));
            foreach (var especialidad in _maestros.ListarEspecialidades())
            {
                cboEspecialidad.Items.Add(new ElementoLista(
                    especialidad.IdEspecialidad.ToString(), especialidad.Nombre));
            }
            cboEspecialidad.SelectedIndex = 0;
        }

        // ===================================================================
        // Eventos
        // ===================================================================

        private void CboPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargando) return;

            string periodo = ValorSeleccionado(cboPeriodo);
            if (periodo == null || periodo == "PERSONALIZADO") return;

            DateTime hoy = DateTime.Today;
            DateTime desde = hoy, hasta = hoy;

            switch (periodo)
            {
                case "HOY":          desde = hoy;                              break;
                case "AYER":         desde = hasta = hoy.AddDays(-1);          break;
                case "SEMANA":       desde = hoy.AddDays(-6);                  break;
                case "QUINCENA":     desde = hoy.AddDays(-14);                 break;
                case "MES":          desde = hoy.AddDays(-29);                 break;
                case "MES_ACTUAL":   desde = new DateTime(hoy.Year, hoy.Month, 1); break;
                case "MES_ANTERIOR":
                    DateTime primeroMesAnterior = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-1);
                    desde = primeroMesAnterior;
                    hasta = primeroMesAnterior.AddMonths(1).AddDays(-1);
                    break;
                case "TRIMESTRE":    desde = hoy.AddMonths(-3);                break;
                case "ANIO":         desde = new DateTime(hoy.Year, 1, 1);     break;
            }

            _cargando = true;
            dtpDesde.Value = desde;
            dtpHasta.Value = hasta;
            _cargando = false;
        }

        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            Generar();
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            _cargando = true;
            cboPeriodo.SelectedIndex = 4;
            cboEstado.SelectedIndex = 0;
            cboMedico.SelectedIndex = 0;
            cboEspecialidad.SelectedIndex = 0;
            txtDocumento.Clear();
            txtCie10.Clear();
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            _cargando = false;

            Generar();
        }

        // ===================================================================
        // Generación
        // ===================================================================

        private void Generar()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                FiltroReporte filtro = ArmarFiltro();
                DataTable datos = _servicio.Generar(filtro);

                MostrarResumen(datos);

                visor.LocalReport.DataSources.Clear();
                CargarDefinicion();
                visor.LocalReport.DataSources.Add(new ReportDataSource(NombreConjuntoDatos, datos));
                visor.LocalReport.SetParameters(new[]
                {
                    new ReportParameter("pInstitucion",
                        ConfigurationManager.AppSettings["NombreInstitucion"] ?? Tema.NombreHospitalCorto),
                    new ReportParameter("pTituloReporte", "REPORTE DE PACIENTES Y ATENCIONES"),
                    new ReportParameter("pFechaDesde", filtro.FechaDesde.ToString("dd/MM/yyyy")),
                    new ReportParameter("pFechaHasta", filtro.FechaHasta.ToString("dd/MM/yyyy")),
                    new ReportParameter("pFiltro", filtro.DescripcionFiltro()),
                    new ReportParameter("pGenerado", DateTime.Now.ToString("dd/MM/yyyy HH:mm")),
                    new ReportParameter("pUsuario", Sesion.NombreCompleto ?? "Sistema")
                });

                visor.RefreshReport();
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

        /// <summary>
        /// Traduce los controles a los criterios que entiende la capa de negocio.
        /// Las descripciones se envían también al reporte, para que la cabecera
        /// impresa deje constancia exacta de con qué filtros se generó.
        /// </summary>
        private FiltroReporte ArmarFiltro()
        {
            return new FiltroReporte
            {
                FechaDesde = dtpDesde.Value.Date,
                FechaHasta = dtpHasta.Value.Date,
                Estado = ValorSeleccionado(cboEstado),
                IdMedico = EnteroSeleccionado(cboMedico),
                IdEspecialidad = EnteroSeleccionado(cboEspecialidad),
                Documento = txtDocumento.Text,
                CodigoCie10 = txtCie10.Text,
                EstadoDescripcion = TextoSeleccionado(cboEstado),
                MedicoDescripcion = EnteroSeleccionado(cboMedico).HasValue ? TextoSeleccionado(cboMedico) : null,
                EspecialidadDescripcion = EnteroSeleccionado(cboEspecialidad).HasValue
                                          ? TextoSeleccionado(cboEspecialidad) : null
            };
        }

        /// <summary>
        /// Totales por estado calculados sobre el mismo conjunto de datos que recibe
        /// el reporte, para que el recuadro de la pantalla y el del PDF no puedan
        /// discrepar.
        /// </summary>
        private void MostrarResumen(DataTable datos)
        {
            if (datos.Rows.Count == 0)
            {
                lblResumen.Text = "Sin registros en el periodo y filtros seleccionados.";
                return;
            }

            lblResumen.Text =
                "Total: " + datos.Rows.Count + "\n" +
                "Atendidos: " + Contar(datos, EstadoCita.Atendido) + "\n" +
                "No atendidos: " + Contar(datos, EstadoCita.NoAtendido) + "\n" +
                "Citados: " + Contar(datos, EstadoCita.Citado) + "\n" +
                "No acudieron: " + Contar(datos, EstadoCita.NoAcudio) + "\n" +
                "Cancelados: " + Contar(datos, EstadoCita.Cancelado);
        }

        private static int Contar(DataTable datos, string estado)
        {
            if (!datos.Columns.Contains("EstadoCita")) return 0;

            return datos.Rows.Cast<DataRow>()
                        .Count(fila => string.Equals(Convert.ToString(fila["EstadoCita"]), estado,
                                                     StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Toma el RDLC incrustado en el ensamblado; si el proyecto se compiló con el
        /// archivo copiado a la carpeta de salida, lo carga desde disco. Tener las dos
        /// vías evita que un cambio en las propiedades del archivo deje el reporte
        /// inutilizable.
        /// </summary>
        private void CargarDefinicion()
        {
            Assembly ensamblado = Assembly.GetExecutingAssembly();

            if (ensamblado.GetManifestResourceNames().Any(nombre => nombre == RecursoIncrustado))
            {
                visor.LocalReport.ReportEmbeddedResource = RecursoIncrustado;
                visor.LocalReport.ReportPath = string.Empty;
                return;
            }

            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", ArchivoReporte);
            if (!File.Exists(ruta))
                throw new NegocioException("No se encontró la definición del reporte (" + ArchivoReporte + ").");

            visor.LocalReport.ReportEmbeddedResource = null;
            visor.LocalReport.ReportPath = ruta;
        }

        // ===================================================================
        // Apoyo
        // ===================================================================

        private static string ValorSeleccionado(ComboBox combo)
        {
            var elemento = combo.SelectedItem as ElementoLista;
            return elemento == null ? null : elemento.Valor;
        }

        private static string TextoSeleccionado(ComboBox combo)
        {
            var elemento = combo.SelectedItem as ElementoLista;
            return elemento == null ? null : elemento.Texto;
        }

        private static int? EnteroSeleccionado(ComboBox combo)
        {
            string valor = ValorSeleccionado(combo);
            int numero;
            return int.TryParse(valor, out numero) ? numero : (int?)null;
        }

        private void ManejarError(Exception ex)
        {
            if (ex is NegocioException)
            {
                Avisos.Advertencia(this, ex.Message);
                return;
            }

            Registro.Error("FrmReportes", ex);
            Avisos.Error(this, "No fue posible generar el reporte.\n\n" + ex.Message);
        }
    }
}
