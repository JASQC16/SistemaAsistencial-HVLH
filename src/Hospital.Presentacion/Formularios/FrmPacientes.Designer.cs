namespace Hospital.Presentacion.Formularios
{
    partial class FrmPacientes
    {
        private System.ComponentModel.IContainer components = null;

        private Hospital.Presentacion.Controles.EncabezadoHvlh encabezado;
        private System.Windows.Forms.GroupBox grpFiltros;
        private System.Windows.Forms.Label lblBusqueda;
        private System.Windows.Forms.TextBox txtBusqueda;
        private System.Windows.Forms.Label lblTipoDocumento;
        private System.Windows.Forms.ComboBox cboTipoDocumento;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnLimpiar;

        private System.Windows.Forms.DataGridView grdPacientes;
        private System.Windows.Forms.Panel pnlAcciones;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnConsultar;
        private System.Windows.Forms.Button btnEstado;
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
            this.lblBusqueda = new System.Windows.Forms.Label();
            this.txtBusqueda = new System.Windows.Forms.TextBox();
            this.lblTipoDocumento = new System.Windows.Forms.Label();
            this.cboTipoDocumento = new System.Windows.Forms.ComboBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.grdPacientes = new System.Windows.Forms.DataGridView();
            this.pnlAcciones = new System.Windows.Forms.Panel();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnConsultar = new System.Windows.Forms.Button();
            this.btnEstado = new System.Windows.Forms.Button();
            this.lblResumen = new System.Windows.Forms.Label();
            this.grpFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPacientes)).BeginInit();
            this.pnlAcciones.SuspendLayout();
            this.SuspendLayout();

            // encabezado
            this.encabezado.Name = "encabezado";
            this.encabezado.Titulo = "Gestión de pacientes";
            this.encabezado.Subtitulo = "Registro único de pacientes: cada persona se registra una sola vez y se reutiliza en todas sus citas y atenciones";

            // ---------------------------------------------------------------
            // grpFiltros
            // ---------------------------------------------------------------
            this.grpFiltros.Controls.Add(this.lblBusqueda);
            this.grpFiltros.Controls.Add(this.txtBusqueda);
            this.grpFiltros.Controls.Add(this.lblTipoDocumento);
            this.grpFiltros.Controls.Add(this.cboTipoDocumento);
            this.grpFiltros.Controls.Add(this.lblEstado);
            this.grpFiltros.Controls.Add(this.cboEstado);
            this.grpFiltros.Controls.Add(this.btnBuscar);
            this.grpFiltros.Controls.Add(this.btnLimpiar);
            this.grpFiltros.Location = new System.Drawing.Point(16, 86);
            this.grpFiltros.Name = "grpFiltros";
            this.grpFiltros.Size = new System.Drawing.Size(1108, 82);
            this.grpFiltros.TabStop = false;
            this.grpFiltros.Text = "Búsqueda";

            this.lblBusqueda.AutoSize = true;
            this.lblBusqueda.Location = new System.Drawing.Point(16, 30);
            this.lblBusqueda.Name = "lblBusqueda";
            this.lblBusqueda.Text = "Documento, historia clínica o nombre";

            this.txtBusqueda.Location = new System.Drawing.Point(16, 48);
            this.txtBusqueda.MaxLength = 100;
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.Size = new System.Drawing.Size(330, 25);
            this.txtBusqueda.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtBusqueda_KeyDown);

            this.lblTipoDocumento.AutoSize = true;
            this.lblTipoDocumento.Location = new System.Drawing.Point(364, 30);
            this.lblTipoDocumento.Name = "lblTipoDocumento";
            this.lblTipoDocumento.Text = "Tipo de documento";

            this.cboTipoDocumento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoDocumento.Location = new System.Drawing.Point(364, 47);
            this.cboTipoDocumento.Name = "cboTipoDocumento";
            this.cboTipoDocumento.Size = new System.Drawing.Size(200, 25);

            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(584, 30);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Text = "Estado";

            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.Location = new System.Drawing.Point(584, 47);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(160, 25);

            this.btnBuscar.Location = new System.Drawing.Point(768, 46);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(110, 28);
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.BtnBuscar_Click);

            this.btnLimpiar.Location = new System.Drawing.Point(886, 46);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(110, 28);
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.Click += new System.EventHandler(this.BtnLimpiar_Click);

            // ---------------------------------------------------------------
            // grdPacientes
            // ---------------------------------------------------------------
            this.grdPacientes.AllowUserToOrderColumns = false;
            this.grdPacientes.AutoGenerateColumns = false;
            this.grdPacientes.Location = new System.Drawing.Point(16, 178);
            this.grdPacientes.Name = "grdPacientes";
            this.grdPacientes.ReadOnly = true;
            this.grdPacientes.Size = new System.Drawing.Size(1108, 366);
            this.grdPacientes.TabIndex = 10;
            this.grdPacientes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GrdPacientes_CellDoubleClick);
            this.grdPacientes.SelectionChanged += new System.EventHandler(this.GrdPacientes_SelectionChanged);

            // ---------------------------------------------------------------
            // pnlAcciones
            // ---------------------------------------------------------------
            this.pnlAcciones.Controls.Add(this.btnNuevo);
            this.pnlAcciones.Controls.Add(this.btnEditar);
            this.pnlAcciones.Controls.Add(this.btnConsultar);
            this.pnlAcciones.Controls.Add(this.btnEstado);
            this.pnlAcciones.Controls.Add(this.lblResumen);
            this.pnlAcciones.Location = new System.Drawing.Point(16, 552);
            this.pnlAcciones.Name = "pnlAcciones";
            this.pnlAcciones.Size = new System.Drawing.Size(1108, 48);

            this.btnNuevo.Location = new System.Drawing.Point(0, 6);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(150, 34);
            this.btnNuevo.Text = "Nuevo paciente";
            this.btnNuevo.Click += new System.EventHandler(this.BtnNuevo_Click);

            this.btnEditar.Location = new System.Drawing.Point(160, 6);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(110, 34);
            this.btnEditar.Text = "Editar";
            this.btnEditar.Click += new System.EventHandler(this.BtnEditar_Click);

            this.btnConsultar.Location = new System.Drawing.Point(280, 6);
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Size = new System.Drawing.Size(110, 34);
            this.btnConsultar.Text = "Consultar";
            this.btnConsultar.Click += new System.EventHandler(this.BtnConsultar_Click);

            this.btnEstado.Location = new System.Drawing.Point(400, 6);
            this.btnEstado.Name = "btnEstado";
            this.btnEstado.Size = new System.Drawing.Size(140, 34);
            this.btnEstado.Text = "Desactivar";
            this.btnEstado.Click += new System.EventHandler(this.BtnEstado_Click);

            this.lblResumen.AutoSize = false;
            this.lblResumen.Location = new System.Drawing.Point(560, 16);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(540, 20);
            this.lblResumen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblResumen.Text = "";

            // ---------------------------------------------------------------
            // FrmPacientes
            // ---------------------------------------------------------------
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1140, 614);
            this.Controls.Add(this.pnlAcciones);
            this.Controls.Add(this.grdPacientes);
            this.Controls.Add(this.grpFiltros);
            this.Controls.Add(this.encabezado);
            this.Name = "FrmPacientes";
            this.Text = "HVLH — Gestión de pacientes";
            this.Load += new System.EventHandler(this.FrmPacientes_Load);

            Hospital.Presentacion.Tema.AplicarFormulario(this);
            Hospital.Presentacion.Tema.Grupo(this.grpFiltros);
            Hospital.Presentacion.Tema.Grilla(this.grdPacientes);
            Hospital.Presentacion.Tema.CampoTexto(this.txtBusqueda);
            Hospital.Presentacion.Tema.CampoTexto(this.cboTipoDocumento);
            Hospital.Presentacion.Tema.CampoTexto(this.cboEstado);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnBuscar);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnLimpiar);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnNuevo);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnEditar);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnConsultar);
            Hospital.Presentacion.Tema.BotonAtencion(this.btnEstado);
            this.pnlAcciones.BackColor = Hospital.Presentacion.Tema.Blanco;

            this.grpFiltros.ResumeLayout(false);
            this.grpFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPacientes)).EndInit();
            this.pnlAcciones.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
