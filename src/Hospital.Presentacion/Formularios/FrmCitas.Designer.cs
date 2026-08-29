namespace Hospital.Presentacion.Formularios
{
    partial class FrmCitas
    {
        private System.ComponentModel.IContainer components = null;

        private Hospital.Presentacion.Controles.EncabezadoHvlh encabezado;
        private System.Windows.Forms.GroupBox grpFiltros;
        private System.Windows.Forms.CheckBox chkRango;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblBusqueda;
        private System.Windows.Forms.TextBox txtBusqueda;
        private System.Windows.Forms.Label lblMedico;
        private System.Windows.Forms.ComboBox cboMedico;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnLimpiar;

        private System.Windows.Forms.DataGridView grdCitas;
        private System.Windows.Forms.Panel pnlAcciones;
        private System.Windows.Forms.Button btnNueva;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnNoAcudio;
        private System.Windows.Forms.Button btnNoAtendido;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblResumen;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.encabezado = new Hospital.Presentacion.Controles.EncabezadoHvlh();
            this.grpFiltros = new System.Windows.Forms.GroupBox();
            this.chkRango = new System.Windows.Forms.CheckBox();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblBusqueda = new System.Windows.Forms.Label();
            this.txtBusqueda = new System.Windows.Forms.TextBox();
            this.lblMedico = new System.Windows.Forms.Label();
            this.cboMedico = new System.Windows.Forms.ComboBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.grdCitas = new System.Windows.Forms.DataGridView();
            this.pnlAcciones = new System.Windows.Forms.Panel();
            this.btnNueva = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnNoAcudio = new System.Windows.Forms.Button();
            this.btnNoAtendido = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblResumen = new System.Windows.Forms.Label();
            this.grpFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdCitas)).BeginInit();
            this.pnlAcciones.SuspendLayout();
            this.SuspendLayout();

            // encabezado
            this.encabezado.Name = "encabezado";
            this.encabezado.Titulo = "Agenda de citas";
            this.encabezado.Subtitulo = "El desenlace de una cita se registra, nunca se deduce: una cita sin atención no es automáticamente una inasistencia";

            // ---------------------------------------------------------------
            // grpFiltros
            // ---------------------------------------------------------------
            this.grpFiltros.Controls.Add(this.chkRango);
            this.grpFiltros.Controls.Add(this.dtpDesde);
            this.grpFiltros.Controls.Add(this.lblHasta);
            this.grpFiltros.Controls.Add(this.dtpHasta);
            this.grpFiltros.Controls.Add(this.lblBusqueda);
            this.grpFiltros.Controls.Add(this.txtBusqueda);
            this.grpFiltros.Controls.Add(this.lblMedico);
            this.grpFiltros.Controls.Add(this.cboMedico);
            this.grpFiltros.Controls.Add(this.lblEstado);
            this.grpFiltros.Controls.Add(this.cboEstado);
            this.grpFiltros.Controls.Add(this.btnBuscar);
            this.grpFiltros.Controls.Add(this.btnLimpiar);
            this.grpFiltros.Location = new System.Drawing.Point(16, 86);
            this.grpFiltros.Name = "grpFiltros";
            this.grpFiltros.Size = new System.Drawing.Size(1152, 126);
            this.grpFiltros.TabStop = false;
            this.grpFiltros.Text = "Filtros de consulta";

            this.chkRango.AutoSize = true;
            this.chkRango.Checked = true;
            this.chkRango.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRango.Location = new System.Drawing.Point(18, 30);
            this.chkRango.Name = "chkRango";
            this.chkRango.Text = "Filtrar por fecha de cita";
            this.chkRango.CheckedChanged += new System.EventHandler(this.ChkRango_CheckedChanged);

            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(18, 52);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(140, 25);

            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(166, 56);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Text = "hasta";

            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(210, 52);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(140, 25);

            this.lblBusqueda.AutoSize = true;
            this.lblBusqueda.Location = new System.Drawing.Point(374, 30);
            this.lblBusqueda.Name = "lblBusqueda";
            this.lblBusqueda.Text = "N.º de cita, documento o paciente";

            this.txtBusqueda.Location = new System.Drawing.Point(374, 52);
            this.txtBusqueda.MaxLength = 100;
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.Size = new System.Drawing.Size(280, 25);
            this.txtBusqueda.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtBusqueda_KeyDown);

            this.lblMedico.AutoSize = true;
            this.lblMedico.Location = new System.Drawing.Point(670, 30);
            this.lblMedico.Name = "lblMedico";
            this.lblMedico.Text = "Profesional";

            this.cboMedico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMedico.Location = new System.Drawing.Point(670, 52);
            this.cboMedico.Name = "cboMedico";
            this.cboMedico.Size = new System.Drawing.Size(280, 25);

            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(966, 30);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Text = "Estado";

            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.Location = new System.Drawing.Point(966, 52);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(168, 25);

            this.btnBuscar.Location = new System.Drawing.Point(914, 86);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(110, 30);
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.BtnBuscar_Click);

            this.btnLimpiar.Location = new System.Drawing.Point(1032, 86);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(102, 30);
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.Click += new System.EventHandler(this.BtnLimpiar_Click);

            // ---------------------------------------------------------------
            // grdCitas
            // ---------------------------------------------------------------
            this.grdCitas.AutoGenerateColumns = false;
            this.grdCitas.Location = new System.Drawing.Point(16, 222);
            this.grdCitas.Name = "grdCitas";
            this.grdCitas.ReadOnly = true;
            this.grdCitas.Size = new System.Drawing.Size(1152, 322);
            this.grdCitas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GrdCitas_CellDoubleClick);
            this.grdCitas.SelectionChanged += new System.EventHandler(this.GrdCitas_SelectionChanged);
            this.grdCitas.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.GrdCitas_RowPrePaint);

            // ---------------------------------------------------------------
            // pnlAcciones
            // ---------------------------------------------------------------
            this.pnlAcciones.Controls.Add(this.btnNueva);
            this.pnlAcciones.Controls.Add(this.btnEditar);
            this.pnlAcciones.Controls.Add(this.btnNoAcudio);
            this.pnlAcciones.Controls.Add(this.btnNoAtendido);
            this.pnlAcciones.Controls.Add(this.btnCancelar);
            this.pnlAcciones.Controls.Add(this.lblResumen);
            this.pnlAcciones.Location = new System.Drawing.Point(16, 552);
            this.pnlAcciones.Name = "pnlAcciones";
            this.pnlAcciones.Size = new System.Drawing.Size(1152, 48);

            this.btnNueva.Location = new System.Drawing.Point(0, 6);
            this.btnNueva.Name = "btnNueva";
            this.btnNueva.Size = new System.Drawing.Size(120, 34);
            this.btnNueva.Text = "Nueva cita";
            this.btnNueva.Click += new System.EventHandler(this.BtnNueva_Click);

            this.btnEditar.Location = new System.Drawing.Point(130, 6);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(130, 34);
            this.btnEditar.Text = "Reprogramar";
            this.btnEditar.Click += new System.EventHandler(this.BtnEditar_Click);

            this.btnNoAcudio.Location = new System.Drawing.Point(270, 6);
            this.btnNoAcudio.Name = "btnNoAcudio";
            this.btnNoAcudio.Size = new System.Drawing.Size(150, 34);
            this.btnNoAcudio.Text = "Marcar no acudió";
            this.btnNoAcudio.Click += new System.EventHandler(this.BtnNoAcudio_Click);

            this.btnNoAtendido.Location = new System.Drawing.Point(430, 6);
            this.btnNoAtendido.Name = "btnNoAtendido";
            this.btnNoAtendido.Size = new System.Drawing.Size(160, 34);
            this.btnNoAtendido.Text = "Marcar no atendido";
            this.btnNoAtendido.Click += new System.EventHandler(this.BtnNoAtendido_Click);

            this.btnCancelar.Location = new System.Drawing.Point(600, 6);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(130, 34);
            this.btnCancelar.Text = "Cancelar cita";
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);

            this.lblResumen.AutoSize = false;
            this.lblResumen.Location = new System.Drawing.Point(742, 14);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(400, 20);
            this.lblResumen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblResumen.Text = "";

            // ---------------------------------------------------------------
            // FrmCitas
            // ---------------------------------------------------------------
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1184, 614);
            this.Controls.Add(this.pnlAcciones);
            this.Controls.Add(this.grdCitas);
            this.Controls.Add(this.grpFiltros);
            this.Controls.Add(this.encabezado);
            this.Name = "FrmCitas";
            this.Text = "HVLH — Agenda de citas";
            this.Load += new System.EventHandler(this.FrmCitas_Load);

            Hospital.Presentacion.Tema.AplicarFormulario(this);
            Hospital.Presentacion.Tema.Grupo(this.grpFiltros);
            Hospital.Presentacion.Tema.Grilla(this.grdCitas);
            Hospital.Presentacion.Tema.CampoTexto(this.txtBusqueda);
            Hospital.Presentacion.Tema.CampoTexto(this.cboMedico);
            Hospital.Presentacion.Tema.CampoTexto(this.cboEstado);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnBuscar);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnLimpiar);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnNueva);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnEditar);
            Hospital.Presentacion.Tema.BotonAtencion(this.btnNoAcudio);
            Hospital.Presentacion.Tema.BotonAtencion(this.btnNoAtendido);
            Hospital.Presentacion.Tema.BotonPeligro(this.btnCancelar);
            this.pnlAcciones.BackColor = Hospital.Presentacion.Tema.Blanco;

            this.grpFiltros.ResumeLayout(false);
            this.grpFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdCitas)).EndInit();
            this.pnlAcciones.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
