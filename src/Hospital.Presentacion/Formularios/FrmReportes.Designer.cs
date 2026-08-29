namespace Hospital.Presentacion.Formularios
{
    partial class FrmReportes
    {
        private System.ComponentModel.IContainer components = null;

        private Hospital.Presentacion.Controles.EncabezadoHvlh encabezado;
        private System.Windows.Forms.GroupBox grpFiltros;

        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblPeriodo;
        private System.Windows.Forms.ComboBox cboPeriodo;

        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Label lblMedico;
        private System.Windows.Forms.ComboBox cboMedico;
        private System.Windows.Forms.Label lblEspecialidad;
        private System.Windows.Forms.ComboBox cboEspecialidad;
        private System.Windows.Forms.Label lblDocumento;
        private System.Windows.Forms.TextBox txtDocumento;
        private System.Windows.Forms.Label lblCie10;
        private System.Windows.Forms.TextBox txtCie10;

        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Label lblResumen;

        private Microsoft.Reporting.WinForms.ReportViewer visor;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.encabezado = new Hospital.Presentacion.Controles.EncabezadoHvlh();
            this.grpFiltros = new System.Windows.Forms.GroupBox();
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblPeriodo = new System.Windows.Forms.Label();
            this.cboPeriodo = new System.Windows.Forms.ComboBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.lblMedico = new System.Windows.Forms.Label();
            this.cboMedico = new System.Windows.Forms.ComboBox();
            this.lblEspecialidad = new System.Windows.Forms.Label();
            this.cboEspecialidad = new System.Windows.Forms.ComboBox();
            this.lblDocumento = new System.Windows.Forms.Label();
            this.txtDocumento = new System.Windows.Forms.TextBox();
            this.lblCie10 = new System.Windows.Forms.Label();
            this.txtCie10 = new System.Windows.Forms.TextBox();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.lblResumen = new System.Windows.Forms.Label();
            this.visor = new Microsoft.Reporting.WinForms.ReportViewer();
            this.grpFiltros.SuspendLayout();
            this.SuspendLayout();

            // encabezado
            this.encabezado.Name = "encabezado";
            this.encabezado.Titulo = "Reporte de pacientes y atenciones";
            this.encabezado.Subtitulo = "Lo que se exporta a PDF, Excel o Word corresponde exactamente a los filtros aplicados";

            // ---------------------------------------------------------------
            // grpFiltros
            // ---------------------------------------------------------------
            this.grpFiltros.Controls.Add(this.lblPeriodo);
            this.grpFiltros.Controls.Add(this.cboPeriodo);
            this.grpFiltros.Controls.Add(this.lblDesde);
            this.grpFiltros.Controls.Add(this.dtpDesde);
            this.grpFiltros.Controls.Add(this.lblHasta);
            this.grpFiltros.Controls.Add(this.dtpHasta);
            this.grpFiltros.Controls.Add(this.lblEstado);
            this.grpFiltros.Controls.Add(this.cboEstado);
            this.grpFiltros.Controls.Add(this.lblMedico);
            this.grpFiltros.Controls.Add(this.cboMedico);
            this.grpFiltros.Controls.Add(this.lblEspecialidad);
            this.grpFiltros.Controls.Add(this.cboEspecialidad);
            this.grpFiltros.Controls.Add(this.lblDocumento);
            this.grpFiltros.Controls.Add(this.txtDocumento);
            this.grpFiltros.Controls.Add(this.lblCie10);
            this.grpFiltros.Controls.Add(this.txtCie10);
            this.grpFiltros.Controls.Add(this.btnGenerar);
            this.grpFiltros.Controls.Add(this.btnLimpiar);
            this.grpFiltros.Controls.Add(this.lblResumen);
            this.grpFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpFiltros.Location = new System.Drawing.Point(0, 74);
            this.grpFiltros.Name = "grpFiltros";
            this.grpFiltros.Padding = new System.Windows.Forms.Padding(16, 6, 16, 6);
            this.grpFiltros.Size = new System.Drawing.Size(1184, 150);
            this.grpFiltros.TabStop = false;
            this.grpFiltros.Text = "Criterios del reporte";

            // --- Primera fila: periodo -------------------------------------
            this.lblPeriodo.AutoSize = true;
            this.lblPeriodo.Location = new System.Drawing.Point(18, 28);
            this.lblPeriodo.Text = "Periodo";

            this.cboPeriodo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPeriodo.Location = new System.Drawing.Point(18, 46);
            this.cboPeriodo.Name = "cboPeriodo";
            this.cboPeriodo.Size = new System.Drawing.Size(190, 25);
            this.cboPeriodo.SelectedIndexChanged += new System.EventHandler(this.CboPeriodo_SelectedIndexChanged);

            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(222, 28);
            this.lblDesde.Text = "Fecha desde *";

            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(222, 46);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(140, 25);

            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(374, 28);
            this.lblHasta.Text = "Fecha hasta *";

            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(374, 46);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(140, 25);

            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(526, 28);
            this.lblEstado.Text = "Estado";

            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.Location = new System.Drawing.Point(526, 46);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(180, 25);

            this.lblMedico.AutoSize = true;
            this.lblMedico.Location = new System.Drawing.Point(718, 28);
            this.lblMedico.Text = "Profesional";

            this.cboMedico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMedico.Location = new System.Drawing.Point(718, 46);
            this.cboMedico.Name = "cboMedico";
            this.cboMedico.Size = new System.Drawing.Size(260, 25);

            // --- Segunda fila: filtros adicionales -------------------------
            this.lblEspecialidad.AutoSize = true;
            this.lblEspecialidad.Location = new System.Drawing.Point(18, 82);
            this.lblEspecialidad.Text = "Servicio / especialidad";

            this.cboEspecialidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEspecialidad.Location = new System.Drawing.Point(18, 100);
            this.cboEspecialidad.Name = "cboEspecialidad";
            this.cboEspecialidad.Size = new System.Drawing.Size(344, 25);

            this.lblDocumento.AutoSize = true;
            this.lblDocumento.Location = new System.Drawing.Point(374, 82);
            this.lblDocumento.Text = "DNI / documento del paciente";

            this.txtDocumento.Location = new System.Drawing.Point(374, 100);
            this.txtDocumento.MaxLength = 15;
            this.txtDocumento.Name = "txtDocumento";
            this.txtDocumento.Size = new System.Drawing.Size(180, 25);
            this.txtDocumento.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblCie10.AutoSize = true;
            this.lblCie10.Location = new System.Drawing.Point(566, 82);
            this.lblCie10.Text = "Diagnóstico CIE-10";

            this.txtCie10.Location = new System.Drawing.Point(566, 100);
            this.txtCie10.MaxLength = 10;
            this.txtCie10.Name = "txtCie10";
            this.txtCie10.Size = new System.Drawing.Size(140, 25);
            this.txtCie10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.btnGenerar.Location = new System.Drawing.Point(718, 98);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(140, 30);
            this.btnGenerar.Text = "Generar reporte";
            this.btnGenerar.Click += new System.EventHandler(this.BtnGenerar_Click);

            this.btnLimpiar.Location = new System.Drawing.Point(868, 98);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(110, 30);
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.Click += new System.EventHandler(this.BtnLimpiar_Click);

            this.lblResumen.AutoSize = false;
            this.lblResumen.Location = new System.Drawing.Point(994, 44);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(176, 86);
            this.lblResumen.Text = "";

            // ---------------------------------------------------------------
            // visor
            // ---------------------------------------------------------------
            this.visor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visor.Location = new System.Drawing.Point(0, 224);
            this.visor.Name = "visor";
            this.visor.ShowBackButton = false;
            this.visor.Size = new System.Drawing.Size(1184, 438);
            this.visor.BackColor = Hospital.Presentacion.Tema.Gris;

            // ---------------------------------------------------------------
            // FrmReportes
            // ---------------------------------------------------------------
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1184, 662);
            this.Controls.Add(this.visor);
            this.Controls.Add(this.grpFiltros);
            this.Controls.Add(this.encabezado);
            this.Name = "FrmReportes";
            this.Text = "HVLH — Reporte de pacientes y atenciones";
            this.Load += new System.EventHandler(this.FrmReportes_Load);

            Hospital.Presentacion.Tema.AplicarFormulario(this);
            Hospital.Presentacion.Tema.Grupo(this.grpFiltros);
            Hospital.Presentacion.Tema.CampoTexto(this.txtDocumento);
            Hospital.Presentacion.Tema.CampoTexto(this.txtCie10);
            Hospital.Presentacion.Tema.CampoTexto(this.cboEstado);
            Hospital.Presentacion.Tema.CampoTexto(this.cboMedico);
            Hospital.Presentacion.Tema.CampoTexto(this.cboEspecialidad);
            Hospital.Presentacion.Tema.CampoTexto(this.cboPeriodo);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnGenerar);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnLimpiar);
            this.lblResumen.ForeColor = Hospital.Presentacion.Tema.TextoSuave;

            this.grpFiltros.ResumeLayout(false);
            this.grpFiltros.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
