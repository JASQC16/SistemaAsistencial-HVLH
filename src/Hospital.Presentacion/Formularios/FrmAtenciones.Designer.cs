namespace Hospital.Presentacion.Formularios
{
    partial class FrmAtenciones
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlTitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbFiltros;
        private System.Windows.Forms.CheckBox chkFechas;
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
        private System.Windows.Forms.DataGridView dgvAtenciones;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.Button btnNueva;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnAnular;
        private System.Windows.Forms.Button btnEliminar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitulo = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.gbFiltros = new System.Windows.Forms.GroupBox();
            this.chkFechas = new System.Windows.Forms.CheckBox();
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
            this.dgvAtenciones = new System.Windows.Forms.DataGridView();
            this.lblResumen = new System.Windows.Forms.Label();
            this.btnNueva = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnAnular = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAtenciones)).BeginInit();
            this.pnlTitulo.SuspendLayout();
            this.gbFiltros.SuspendLayout();
            this.SuspendLayout();

            // pnlTitulo
            this.pnlTitulo.BackColor = Hospital.Presentacion.Tema.AzulProfundo;
            this.pnlTitulo.Controls.Add(this.lblTitulo);
            this.pnlTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitulo.Location = new System.Drawing.Point(0, 0);
            this.pnlTitulo.Name = "pnlTitulo";
            this.pnlTitulo.Size = new System.Drawing.Size(1060, 52);

            this.lblTitulo.AutoSize = false;
            this.lblTitulo.Font = Hospital.Presentacion.Tema.FuenteTitulo;
            this.lblTitulo.ForeColor = Hospital.Presentacion.Tema.Blanco;
            this.lblTitulo.Location = new System.Drawing.Point(20, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(700, 28);
            this.lblTitulo.Text = "Atenciones ambulatorias";

            // gbFiltros
            this.gbFiltros.Controls.Add(this.chkFechas);
            this.gbFiltros.Controls.Add(this.dtpDesde);
            this.gbFiltros.Controls.Add(this.lblHasta);
            this.gbFiltros.Controls.Add(this.dtpHasta);
            this.gbFiltros.Controls.Add(this.lblBusqueda);
            this.gbFiltros.Controls.Add(this.txtBusqueda);
            this.gbFiltros.Controls.Add(this.lblMedico);
            this.gbFiltros.Controls.Add(this.cboMedico);
            this.gbFiltros.Controls.Add(this.lblEstado);
            this.gbFiltros.Controls.Add(this.cboEstado);
            this.gbFiltros.Controls.Add(this.btnBuscar);
            this.gbFiltros.Controls.Add(this.btnLimpiar);
            this.gbFiltros.Location = new System.Drawing.Point(18, 66);
            this.gbFiltros.Name = "gbFiltros";
            this.gbFiltros.Size = new System.Drawing.Size(1024, 122);
            this.gbFiltros.TabStop = false;
            this.gbFiltros.Text = "Filtros de búsqueda";

            // Fila 1
            this.chkFechas.AutoSize = true;
            this.chkFechas.Checked = true;
            this.chkFechas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFechas.Location = new System.Drawing.Point(18, 36);
            this.chkFechas.Name = "chkFechas";
            this.chkFechas.Size = new System.Drawing.Size(110, 21);
            this.chkFechas.Text = "Atendidas desde";
            this.chkFechas.CheckedChanged += new System.EventHandler(this.ChkFechas_CheckedChanged);

            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(140, 33);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(120, 25);

            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(268, 37);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(40, 17);
            this.lblHasta.Text = "hasta";

            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(312, 33);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(120, 25);

            this.lblBusqueda.AutoSize = true;
            this.lblBusqueda.Location = new System.Drawing.Point(456, 37);
            this.lblBusqueda.Name = "lblBusqueda";
            this.lblBusqueda.Size = new System.Drawing.Size(140, 17);
            this.lblBusqueda.Text = "N° atención, DNI o paciente";

            this.txtBusqueda.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBusqueda.Location = new System.Drawing.Point(632, 33);
            this.txtBusqueda.MaxLength = 100;
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.Size = new System.Drawing.Size(240, 25);
            this.txtBusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtBusqueda_KeyDown);

            // Fila 2
            this.lblMedico.AutoSize = true;
            this.lblMedico.Location = new System.Drawing.Point(18, 78);
            this.lblMedico.Name = "lblMedico";
            this.lblMedico.Size = new System.Drawing.Size(50, 17);
            this.lblMedico.Text = "Médico";

            this.cboMedico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMedico.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMedico.Location = new System.Drawing.Point(140, 75);
            this.cboMedico.Name = "cboMedico";
            this.cboMedico.Size = new System.Drawing.Size(292, 25);

            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(456, 78);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(45, 17);
            this.lblEstado.Text = "Estado";

            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboEstado.Location = new System.Drawing.Point(632, 75);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(240, 25);

            this.btnBuscar.Location = new System.Drawing.Point(890, 33);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(116, 32);
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.BtnBuscar_Click);

            this.btnLimpiar.Location = new System.Drawing.Point(890, 72);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(116, 30);
            this.btnLimpiar.Text = "Limpiar filtros";
            this.btnLimpiar.Click += new System.EventHandler(this.BtnLimpiar_Click);

            // dgvAtenciones
            this.dgvAtenciones.AutoGenerateColumns = false;
            this.dgvAtenciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAtenciones.Location = new System.Drawing.Point(18, 200);
            this.dgvAtenciones.Name = "dgvAtenciones";
            this.dgvAtenciones.ReadOnly = true;
            this.dgvAtenciones.Size = new System.Drawing.Size(1024, 372);
            this.dgvAtenciones.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvAtenciones_CellDoubleClick);
            this.dgvAtenciones.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvAtenciones_CellFormatting);

            this.dgvAtenciones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                Columna("colNumero", "NumeroAtencion", "N° atención", 90, false),
                Columna("colFecha", "FechaAtencion", "Fecha", 110, false, "dd/MM/yyyy HH:mm"),
                Columna("colDocumento", "DocumentoPaciente", "Documento", 80, false),
                Columna("colPaciente", "Paciente", "Paciente", 200, true),
                Columna("colEdad", "EdadPaciente", "Edad", 45, false),
                Columna("colMedico", "Medico", "Médico", 160, true),
                Columna("colEspecialidad", "Especialidad", "Especialidad", 120, false),
                Columna("colMotivo", "MotivoConsulta", "Motivo de consulta", 220, true),
                Columna("colDx", "TotalDiagnosticos", "Dx", 40, false),
                Columna("colEstado", "EstadoDescripcion", "Estado", 90, false)});

            // lblResumen
            this.lblResumen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblResumen.AutoSize = false;
            this.lblResumen.ForeColor = Hospital.Presentacion.Tema.TextoSuave;
            this.lblResumen.Location = new System.Drawing.Point(18, 586);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(420, 30);
            this.lblResumen.Text = "";

            // Botones de acción
            this.btnNueva.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNueva.Location = new System.Drawing.Point(566, 582);
            this.btnNueva.Name = "btnNueva";
            this.btnNueva.Size = new System.Drawing.Size(140, 36);
            this.btnNueva.Text = "Nueva atención";
            this.btnNueva.Click += new System.EventHandler(this.BtnNueva_Click);

            this.btnEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditar.Location = new System.Drawing.Point(714, 582);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(100, 36);
            this.btnEditar.Text = "Editar";
            this.btnEditar.Click += new System.EventHandler(this.BtnEditar_Click);

            this.btnAnular.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnular.Location = new System.Drawing.Point(822, 582);
            this.btnAnular.Name = "btnAnular";
            this.btnAnular.Size = new System.Drawing.Size(100, 36);
            this.btnAnular.Text = "Anular";
            this.btnAnular.Click += new System.EventHandler(this.BtnAnular_Click);

            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminar.Location = new System.Drawing.Point(930, 582);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(112, 36);
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.BtnEliminar_Click);

            // FrmAtenciones
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1060, 634);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnAnular);
            this.Controls.Add(this.btnEditar);
            this.Controls.Add(this.btnNueva);
            this.Controls.Add(this.lblResumen);
            this.Controls.Add(this.dgvAtenciones);
            this.Controls.Add(this.gbFiltros);
            this.Controls.Add(this.pnlTitulo);
            this.Name = "FrmAtenciones";
            this.Text = "Gestión de atenciones";

            Hospital.Presentacion.Tema.AplicarFormulario(this);
            Hospital.Presentacion.Tema.Grupo(this.gbFiltros);
            Hospital.Presentacion.Tema.Grilla(this.dgvAtenciones);
            Hospital.Presentacion.Tema.CampoTexto(this.txtBusqueda);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnBuscar);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnLimpiar);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnNueva);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnEditar);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnAnular);
            Hospital.Presentacion.Tema.BotonPeligro(this.btnEliminar);

            ((System.ComponentModel.ISupportInitialize)(this.dgvAtenciones)).EndInit();
            this.pnlTitulo.ResumeLayout(false);
            this.gbFiltros.ResumeLayout(false);
            this.gbFiltros.PerformLayout();
            this.ResumeLayout(false);
        }

        /// <summary>Fábrica de columnas: mantiene la definición de la grilla legible.</summary>
        private static System.Windows.Forms.DataGridViewTextBoxColumn Columna(
            string nombre, string propiedad, string titulo, int peso, bool ajustar, string formato = null)
        {
            var columna = new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = nombre,
                DataPropertyName = propiedad,
                HeaderText = titulo,
                FillWeight = peso,
                ReadOnly = true,
                SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
            };
            if (formato != null) columna.DefaultCellStyle.Format = formato;
            if (!ajustar) columna.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            return columna;
        }
    }
}
