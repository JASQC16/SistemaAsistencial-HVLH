namespace Hospital.Presentacion.Formularios
{
    partial class FrmAtencionEdicion
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlTitulo;
        private System.Windows.Forms.Label lblTitulo;

        private System.Windows.Forms.GroupBox gbCabecera;
        private System.Windows.Forms.Label lblDocumento;
        private System.Windows.Forms.TextBox txtDocumento;
        private System.Windows.Forms.Button btnBuscarPaciente;
        private System.Windows.Forms.Label lblPaciente;
        private System.Windows.Forms.Label lblMedico;
        private System.Windows.Forms.ComboBox cboMedico;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Label lblCita;
        private System.Windows.Forms.ComboBox cboCita;
        private System.Windows.Forms.Label lblAyudaCita;
        private System.Windows.Forms.Label lblMotivo;
        private System.Windows.Forms.TextBox txtMotivo;
        private System.Windows.Forms.Label lblSignos;
        private System.Windows.Forms.Label lblTemp;
        private System.Windows.Forms.TextBox txtTemperatura;
        private System.Windows.Forms.Label lblPresionEt;
        private System.Windows.Forms.TextBox txtPresion;
        private System.Windows.Forms.Label lblFrecEt;
        private System.Windows.Forms.TextBox txtFrecuencia;
        private System.Windows.Forms.Label lblPesoEt;
        private System.Windows.Forms.TextBox txtPeso;
        private System.Windows.Forms.Label lblTallaEt;
        private System.Windows.Forms.TextBox txtTalla;
        private System.Windows.Forms.Label lblUnidades;
        private System.Windows.Forms.Label lblObservaciones;
        private System.Windows.Forms.TextBox txtObservaciones;

        private System.Windows.Forms.GroupBox gbDetalle;
        private System.Windows.Forms.Label lblBuscarCie;
        private System.Windows.Forms.TextBox txtBuscarCie;
        private System.Windows.Forms.Button btnBuscarCie;
        private System.Windows.Forms.Label lblEstadoApi;
        private System.Windows.Forms.ComboBox cboResultadosCie;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.ComboBox cboTipoDiagnostico;
        private System.Windows.Forms.Button btnAgregarDetalle;
        private System.Windows.Forms.Label lblIndicaciones;
        private System.Windows.Forms.TextBox txtIndicaciones;
        private System.Windows.Forms.Label lblManual;
        private System.Windows.Forms.TextBox txtCodigoManual;
        private System.Windows.Forms.TextBox txtDescripcionManual;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Label lblTotalDetalle;
        private System.Windows.Forms.Button btnQuitarDetalle;

        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitulo = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.gbCabecera = new System.Windows.Forms.GroupBox();
            this.lblDocumento = new System.Windows.Forms.Label();
            this.txtDocumento = new System.Windows.Forms.TextBox();
            this.btnBuscarPaciente = new System.Windows.Forms.Button();
            this.lblPaciente = new System.Windows.Forms.Label();
            this.lblMedico = new System.Windows.Forms.Label();
            this.cboMedico = new System.Windows.Forms.ComboBox();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lblEstado = new System.Windows.Forms.Label();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.lblCita = new System.Windows.Forms.Label();
            this.cboCita = new System.Windows.Forms.ComboBox();
            this.lblAyudaCita = new System.Windows.Forms.Label();
            this.lblMotivo = new System.Windows.Forms.Label();
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.lblSignos = new System.Windows.Forms.Label();
            this.lblTemp = new System.Windows.Forms.Label();
            this.txtTemperatura = new System.Windows.Forms.TextBox();
            this.lblPresionEt = new System.Windows.Forms.Label();
            this.txtPresion = new System.Windows.Forms.TextBox();
            this.lblFrecEt = new System.Windows.Forms.Label();
            this.txtFrecuencia = new System.Windows.Forms.TextBox();
            this.lblPesoEt = new System.Windows.Forms.Label();
            this.txtPeso = new System.Windows.Forms.TextBox();
            this.lblTallaEt = new System.Windows.Forms.Label();
            this.txtTalla = new System.Windows.Forms.TextBox();
            this.lblUnidades = new System.Windows.Forms.Label();
            this.lblObservaciones = new System.Windows.Forms.Label();
            this.txtObservaciones = new System.Windows.Forms.TextBox();
            this.gbDetalle = new System.Windows.Forms.GroupBox();
            this.lblBuscarCie = new System.Windows.Forms.Label();
            this.txtBuscarCie = new System.Windows.Forms.TextBox();
            this.btnBuscarCie = new System.Windows.Forms.Button();
            this.lblEstadoApi = new System.Windows.Forms.Label();
            this.cboResultadosCie = new System.Windows.Forms.ComboBox();
            this.lblTipo = new System.Windows.Forms.Label();
            this.cboTipoDiagnostico = new System.Windows.Forms.ComboBox();
            this.btnAgregarDetalle = new System.Windows.Forms.Button();
            this.lblIndicaciones = new System.Windows.Forms.Label();
            this.txtIndicaciones = new System.Windows.Forms.TextBox();
            this.lblManual = new System.Windows.Forms.Label();
            this.txtCodigoManual = new System.Windows.Forms.TextBox();
            this.txtDescripcionManual = new System.Windows.Forms.TextBox();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.lblTotalDetalle = new System.Windows.Forms.Label();
            this.btnQuitarDetalle = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.pnlTitulo.SuspendLayout();
            this.gbCabecera.SuspendLayout();
            this.gbDetalle.SuspendLayout();
            this.SuspendLayout();

            // ------------------------------- Encabezado -------------------------------
            this.pnlTitulo.BackColor = Hospital.Presentacion.Tema.AzulProfundo;
            this.pnlTitulo.Controls.Add(this.lblTitulo);
            this.pnlTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitulo.Location = new System.Drawing.Point(0, 0);
            this.pnlTitulo.Name = "pnlTitulo";
            this.pnlTitulo.Size = new System.Drawing.Size(944, 52);

            this.lblTitulo.AutoSize = false;
            this.lblTitulo.Font = Hospital.Presentacion.Tema.FuenteTitulo;
            this.lblTitulo.ForeColor = Hospital.Presentacion.Tema.Blanco;
            this.lblTitulo.Location = new System.Drawing.Point(20, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(700, 28);
            this.lblTitulo.Text = "Nueva atención ambulatoria";

            // -------------------------------- Cabecera --------------------------------
            this.gbCabecera.Controls.Add(this.lblDocumento);
            this.gbCabecera.Controls.Add(this.txtDocumento);
            this.gbCabecera.Controls.Add(this.btnBuscarPaciente);
            this.gbCabecera.Controls.Add(this.lblPaciente);
            this.gbCabecera.Controls.Add(this.lblMedico);
            this.gbCabecera.Controls.Add(this.cboMedico);
            this.gbCabecera.Controls.Add(this.lblFecha);
            this.gbCabecera.Controls.Add(this.dtpFecha);
            this.gbCabecera.Controls.Add(this.lblEstado);
            this.gbCabecera.Controls.Add(this.cboEstado);
            this.gbCabecera.Controls.Add(this.lblCita);
            this.gbCabecera.Controls.Add(this.cboCita);
            this.gbCabecera.Controls.Add(this.lblAyudaCita);
            this.gbCabecera.Controls.Add(this.lblMotivo);
            this.gbCabecera.Controls.Add(this.txtMotivo);
            this.gbCabecera.Controls.Add(this.lblSignos);
            this.gbCabecera.Controls.Add(this.lblTemp);
            this.gbCabecera.Controls.Add(this.txtTemperatura);
            this.gbCabecera.Controls.Add(this.lblPresionEt);
            this.gbCabecera.Controls.Add(this.txtPresion);
            this.gbCabecera.Controls.Add(this.lblFrecEt);
            this.gbCabecera.Controls.Add(this.txtFrecuencia);
            this.gbCabecera.Controls.Add(this.lblPesoEt);
            this.gbCabecera.Controls.Add(this.txtPeso);
            this.gbCabecera.Controls.Add(this.lblTallaEt);
            this.gbCabecera.Controls.Add(this.txtTalla);
            this.gbCabecera.Controls.Add(this.lblUnidades);
            this.gbCabecera.Controls.Add(this.lblObservaciones);
            this.gbCabecera.Controls.Add(this.txtObservaciones);
            this.gbCabecera.Location = new System.Drawing.Point(16, 64);
            this.gbCabecera.Name = "gbCabecera";
            this.gbCabecera.Size = new System.Drawing.Size(912, 272);
            this.gbCabecera.TabStop = false;
            this.gbCabecera.Text = "Datos de la atención";

            this.lblDocumento.AutoSize = true;
            this.lblDocumento.Location = new System.Drawing.Point(16, 33);
            this.lblDocumento.Name = "lblDocumento";
            this.lblDocumento.Size = new System.Drawing.Size(90, 17);
            this.lblDocumento.Text = "Paciente *";

            this.txtDocumento.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDocumento.Location = new System.Drawing.Point(120, 30);
            this.txtDocumento.MaxLength = 15;
            this.txtDocumento.Name = "txtDocumento";
            this.txtDocumento.ReadOnly = true;
            this.txtDocumento.Size = new System.Drawing.Size(110, 25);
            this.txtDocumento.TabIndex = 0;

            this.btnBuscarPaciente.Location = new System.Drawing.Point(236, 29);
            this.btnBuscarPaciente.Name = "btnBuscarPaciente";
            this.btnBuscarPaciente.Size = new System.Drawing.Size(38, 27);
            this.btnBuscarPaciente.TabIndex = 1;
            this.btnBuscarPaciente.Text = "...";
            this.btnBuscarPaciente.Click += new System.EventHandler(this.BtnBuscarPaciente_Click);

            this.lblPaciente.AutoSize = false;
            this.lblPaciente.ForeColor = Hospital.Presentacion.Tema.Azul;
            this.lblPaciente.Font = Hospital.Presentacion.Tema.FuenteEtiqueta;
            this.lblPaciente.Location = new System.Drawing.Point(284, 34);
            this.lblPaciente.Name = "lblPaciente";
            this.lblPaciente.Size = new System.Drawing.Size(610, 20);
            this.lblPaciente.Text = "Ningún paciente seleccionado";

            this.lblMedico.AutoSize = true;
            this.lblMedico.Location = new System.Drawing.Point(16, 71);
            this.lblMedico.Name = "lblMedico";
            this.lblMedico.Size = new System.Drawing.Size(60, 17);
            this.lblMedico.Text = "Médico *";

            this.cboMedico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMedico.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMedico.Location = new System.Drawing.Point(120, 68);
            this.cboMedico.Name = "cboMedico";
            this.cboMedico.Size = new System.Drawing.Size(300, 25);
            this.cboMedico.TabIndex = 2;

            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(440, 71);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(50, 17);
            this.lblFecha.Text = "Fecha *";

            this.dtpFecha.CustomFormat = "dd/MM/yyyy  HH:mm";
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha.Location = new System.Drawing.Point(500, 68);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(180, 25);
            this.dtpFecha.TabIndex = 3;

            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(700, 71);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(45, 17);
            this.lblEstado.Text = "Estado";

            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboEstado.Location = new System.Drawing.Point(760, 68);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(134, 25);
            this.cboEstado.TabIndex = 4;

            // Cita de origen: enlaza el acto clinico con la cita programada. Puede
            // quedar vacio, porque una atencion tambien puede ser demanda espontanea.
            this.lblCita.AutoSize = true;
            this.lblCita.Location = new System.Drawing.Point(16, 109);
            this.lblCita.Name = "lblCita";
            this.lblCita.Size = new System.Drawing.Size(95, 17);
            this.lblCita.Text = "Cita de origen";

            this.cboCita.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCita.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboCita.Location = new System.Drawing.Point(120, 106);
            this.cboCita.Name = "cboCita";
            this.cboCita.Size = new System.Drawing.Size(430, 25);
            this.cboCita.TabIndex = 5;

            this.lblAyudaCita.AutoSize = false;
            this.lblAyudaCita.ForeColor = Hospital.Presentacion.Tema.TextoSuave;
            this.lblAyudaCita.Location = new System.Drawing.Point(560, 109);
            this.lblAyudaCita.Name = "lblAyudaCita";
            this.lblAyudaCita.Size = new System.Drawing.Size(334, 34);
            this.lblAyudaCita.Text = "Al guardar, la cita seleccionada pasa a ATENDIDO.";

            this.lblMotivo.AutoSize = true;
            this.lblMotivo.Location = new System.Drawing.Point(16, 145);
            this.lblMotivo.Name = "lblMotivo";
            this.lblMotivo.Size = new System.Drawing.Size(90, 17);
            this.lblMotivo.Text = "Motivo *";

            this.txtMotivo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMotivo.Location = new System.Drawing.Point(120, 142);
            this.txtMotivo.MaxLength = 300;
            this.txtMotivo.Multiline = true;
            this.txtMotivo.Name = "txtMotivo";
            this.txtMotivo.Size = new System.Drawing.Size(774, 44);
            this.txtMotivo.TabIndex = 5;

            this.lblSignos.AutoSize = true;
            this.lblSignos.Font = Hospital.Presentacion.Tema.FuenteEtiqueta;
            this.lblSignos.ForeColor = Hospital.Presentacion.Tema.Azul;
            this.lblSignos.Location = new System.Drawing.Point(16, 199);
            this.lblSignos.Name = "lblSignos";
            this.lblSignos.Size = new System.Drawing.Size(90, 17);
            this.lblSignos.Text = "Signos vitales";

            this.lblTemp.AutoSize = true;
            this.lblTemp.Location = new System.Drawing.Point(120, 199);
            this.lblTemp.Name = "lblTemp";
            this.lblTemp.Size = new System.Drawing.Size(24, 17);
            this.lblTemp.Text = "T°";

            this.txtTemperatura.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTemperatura.Location = new System.Drawing.Point(148, 196);
            this.txtTemperatura.MaxLength = 5;
            this.txtTemperatura.Name = "txtTemperatura";
            this.txtTemperatura.Size = new System.Drawing.Size(54, 25);
            this.txtTemperatura.TabIndex = 6;

            this.lblPresionEt.AutoSize = true;
            this.lblPresionEt.Location = new System.Drawing.Point(214, 199);
            this.lblPresionEt.Name = "lblPresionEt";
            this.lblPresionEt.Size = new System.Drawing.Size(26, 17);
            this.lblPresionEt.Text = "PA";

            this.txtPresion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPresion.Location = new System.Drawing.Point(244, 196);
            this.txtPresion.MaxLength = 10;
            this.txtPresion.Name = "txtPresion";
            this.txtPresion.Size = new System.Drawing.Size(70, 25);
            this.txtPresion.TabIndex = 7;

            this.lblFrecEt.AutoSize = true;
            this.lblFrecEt.Location = new System.Drawing.Point(328, 199);
            this.lblFrecEt.Name = "lblFrecEt";
            this.lblFrecEt.Size = new System.Drawing.Size(26, 17);
            this.lblFrecEt.Text = "FC";

            this.txtFrecuencia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFrecuencia.Location = new System.Drawing.Point(358, 196);
            this.txtFrecuencia.MaxLength = 3;
            this.txtFrecuencia.Name = "txtFrecuencia";
            this.txtFrecuencia.Size = new System.Drawing.Size(54, 25);
            this.txtFrecuencia.TabIndex = 8;

            this.lblPesoEt.AutoSize = true;
            this.lblPesoEt.Location = new System.Drawing.Point(424, 199);
            this.lblPesoEt.Name = "lblPesoEt";
            this.lblPesoEt.Size = new System.Drawing.Size(34, 17);
            this.lblPesoEt.Text = "Peso";

            this.txtPeso.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPeso.Location = new System.Drawing.Point(464, 196);
            this.txtPeso.MaxLength = 6;
            this.txtPeso.Name = "txtPeso";
            this.txtPeso.Size = new System.Drawing.Size(58, 25);
            this.txtPeso.TabIndex = 9;

            this.lblTallaEt.AutoSize = true;
            this.lblTallaEt.Location = new System.Drawing.Point(532, 199);
            this.lblTallaEt.Name = "lblTallaEt";
            this.lblTallaEt.Size = new System.Drawing.Size(34, 17);
            this.lblTallaEt.Text = "Talla";

            this.txtTalla.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTalla.Location = new System.Drawing.Point(572, 196);
            this.txtTalla.MaxLength = 5;
            this.txtTalla.Name = "txtTalla";
            this.txtTalla.Size = new System.Drawing.Size(58, 25);
            this.txtTalla.TabIndex = 10;

            this.lblUnidades.AutoSize = false;
            this.lblUnidades.ForeColor = Hospital.Presentacion.Tema.TextoSuave;
            this.lblUnidades.Location = new System.Drawing.Point(640, 199);
            this.lblUnidades.Name = "lblUnidades";
            this.lblUnidades.Size = new System.Drawing.Size(254, 18);
            this.lblUnidades.Text = "°C · mmHg (120/80) · lpm · kg · metros";

            this.lblObservaciones.AutoSize = true;
            this.lblObservaciones.Location = new System.Drawing.Point(16, 235);
            this.lblObservaciones.Name = "lblObservaciones";
            this.lblObservaciones.Size = new System.Drawing.Size(90, 17);
            this.lblObservaciones.Text = "Observaciones";

            this.txtObservaciones.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtObservaciones.Location = new System.Drawing.Point(120, 232);
            this.txtObservaciones.MaxLength = 500;
            this.txtObservaciones.Name = "txtObservaciones";
            this.txtObservaciones.Size = new System.Drawing.Size(774, 25);
            this.txtObservaciones.TabIndex = 11;

            // --------------------------------- Detalle ---------------------------------
            this.gbDetalle.Controls.Add(this.lblBuscarCie);
            this.gbDetalle.Controls.Add(this.txtBuscarCie);
            this.gbDetalle.Controls.Add(this.btnBuscarCie);
            this.gbDetalle.Controls.Add(this.lblEstadoApi);
            this.gbDetalle.Controls.Add(this.cboResultadosCie);
            this.gbDetalle.Controls.Add(this.lblTipo);
            this.gbDetalle.Controls.Add(this.cboTipoDiagnostico);
            this.gbDetalle.Controls.Add(this.btnAgregarDetalle);
            this.gbDetalle.Controls.Add(this.lblIndicaciones);
            this.gbDetalle.Controls.Add(this.txtIndicaciones);
            this.gbDetalle.Controls.Add(this.lblManual);
            this.gbDetalle.Controls.Add(this.txtCodigoManual);
            this.gbDetalle.Controls.Add(this.txtDescripcionManual);
            this.gbDetalle.Controls.Add(this.dgvDetalle);
            this.gbDetalle.Controls.Add(this.lblTotalDetalle);
            this.gbDetalle.Controls.Add(this.btnQuitarDetalle);
            this.gbDetalle.Location = new System.Drawing.Point(16, 346);
            this.gbDetalle.Name = "gbDetalle";
            this.gbDetalle.Size = new System.Drawing.Size(912, 336);
            this.gbDetalle.TabStop = false;
            this.gbDetalle.Text = "Diagnósticos (detalle de la atención)";

            this.lblBuscarCie.AutoSize = true;
            this.lblBuscarCie.Location = new System.Drawing.Point(16, 33);
            this.lblBuscarCie.Name = "lblBuscarCie";
            this.lblBuscarCie.Size = new System.Drawing.Size(140, 17);
            this.lblBuscarCie.Text = "Buscar en catálogo CIE-10";

            this.txtBuscarCie.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBuscarCie.Location = new System.Drawing.Point(180, 30);
            this.txtBuscarCie.MaxLength = 60;
            this.txtBuscarCie.Name = "txtBuscarCie";
            this.txtBuscarCie.Size = new System.Drawing.Size(230, 25);
            this.txtBuscarCie.TabIndex = 12;
            this.txtBuscarCie.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtBuscarCie_KeyDown);

            this.btnBuscarCie.Location = new System.Drawing.Point(418, 29);
            this.btnBuscarCie.Name = "btnBuscarCie";
            this.btnBuscarCie.Size = new System.Drawing.Size(120, 28);
            this.btnBuscarCie.TabIndex = 13;
            this.btnBuscarCie.Text = "Buscar";
            this.btnBuscarCie.Click += new System.EventHandler(this.BtnBuscarCie_Click);

            this.lblEstadoApi.AutoSize = false;
            this.lblEstadoApi.ForeColor = Hospital.Presentacion.Tema.TextoSuave;
            this.lblEstadoApi.Location = new System.Drawing.Point(548, 34);
            this.lblEstadoApi.Name = "lblEstadoApi";
            this.lblEstadoApi.Size = new System.Drawing.Size(346, 20);
            this.lblEstadoApi.Text = "";

            this.cboResultadosCie.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboResultadosCie.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboResultadosCie.Location = new System.Drawing.Point(16, 66);
            this.cboResultadosCie.Name = "cboResultadosCie";
            this.cboResultadosCie.Size = new System.Drawing.Size(522, 25);
            this.cboResultadosCie.TabIndex = 14;

            this.lblTipo.AutoSize = true;
            this.lblTipo.Location = new System.Drawing.Point(548, 69);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(34, 17);
            this.lblTipo.Text = "Tipo";

            this.cboTipoDiagnostico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoDiagnostico.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboTipoDiagnostico.Location = new System.Drawing.Point(590, 66);
            this.cboTipoDiagnostico.Name = "cboTipoDiagnostico";
            this.cboTipoDiagnostico.Size = new System.Drawing.Size(130, 25);
            this.cboTipoDiagnostico.TabIndex = 15;

            this.btnAgregarDetalle.Location = new System.Drawing.Point(736, 65);
            this.btnAgregarDetalle.Name = "btnAgregarDetalle";
            this.btnAgregarDetalle.Size = new System.Drawing.Size(158, 28);
            this.btnAgregarDetalle.TabIndex = 16;
            this.btnAgregarDetalle.Text = "Agregar diagnóstico";
            this.btnAgregarDetalle.Click += new System.EventHandler(this.BtnAgregarDetalle_Click);

            this.lblIndicaciones.AutoSize = true;
            this.lblIndicaciones.Location = new System.Drawing.Point(16, 103);
            this.lblIndicaciones.Name = "lblIndicaciones";
            this.lblIndicaciones.Size = new System.Drawing.Size(85, 17);
            this.lblIndicaciones.Text = "Indicaciones";

            this.txtIndicaciones.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIndicaciones.Location = new System.Drawing.Point(180, 100);
            this.txtIndicaciones.MaxLength = 300;
            this.txtIndicaciones.Name = "txtIndicaciones";
            this.txtIndicaciones.Size = new System.Drawing.Size(358, 25);
            this.txtIndicaciones.TabIndex = 17;

            this.lblManual.AutoSize = false;
            this.lblManual.ForeColor = Hospital.Presentacion.Tema.TextoSuave;
            this.lblManual.Location = new System.Drawing.Point(548, 103);
            this.lblManual.Name = "lblManual";
            this.lblManual.Size = new System.Drawing.Size(64, 18);
            this.lblManual.Text = "o manual:";

            this.txtCodigoManual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCodigoManual.Location = new System.Drawing.Point(614, 100);
            this.txtCodigoManual.MaxLength = 10;
            this.txtCodigoManual.Name = "txtCodigoManual";
            this.txtCodigoManual.Size = new System.Drawing.Size(76, 25);
            this.txtCodigoManual.TabIndex = 18;

            this.txtDescripcionManual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDescripcionManual.Location = new System.Drawing.Point(696, 100);
            this.txtDescripcionManual.MaxLength = 250;
            this.txtDescripcionManual.Name = "txtDescripcionManual";
            this.txtDescripcionManual.Size = new System.Drawing.Size(198, 25);
            this.txtDescripcionManual.TabIndex = 19;

            this.dgvDetalle.AutoGenerateColumns = false;
            this.dgvDetalle.Location = new System.Drawing.Point(16, 136);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.Size = new System.Drawing.Size(878, 140);
            this.dgvDetalle.TabIndex = 20;
            this.dgvDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                ColumnaDetalle("Item", "N°", 30),
                ColumnaDetalle("CodigoCie10", "Código", 60),
                ColumnaDetalle("DescripcionDiagnostico", "Diagnóstico", 300),
                ColumnaDetalle("TipoDiagnosticoDescripcion", "Tipo", 90),
                ColumnaDetalle("Indicaciones", "Indicaciones", 220)});

            this.lblTotalDetalle.AutoSize = false;
            this.lblTotalDetalle.ForeColor = Hospital.Presentacion.Tema.TextoSuave;
            this.lblTotalDetalle.Location = new System.Drawing.Point(16, 290);
            this.lblTotalDetalle.Name = "lblTotalDetalle";
            this.lblTotalDetalle.Size = new System.Drawing.Size(420, 20);
            this.lblTotalDetalle.Text = "Sin diagnósticos registrados.";

            this.btnQuitarDetalle.Location = new System.Drawing.Point(768, 284);
            this.btnQuitarDetalle.Name = "btnQuitarDetalle";
            this.btnQuitarDetalle.Size = new System.Drawing.Size(126, 30);
            this.btnQuitarDetalle.TabIndex = 21;
            this.btnQuitarDetalle.Text = "Quitar del detalle";
            this.btnQuitarDetalle.Click += new System.EventHandler(this.BtnQuitarDetalle_Click);

            // ---------------------------- Acciones del pie -----------------------------
            this.btnGuardar.Location = new System.Drawing.Point(660, 696);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(160, 38);
            this.btnGuardar.TabIndex = 22;
            this.btnGuardar.Text = "Guardar atención";
            this.btnGuardar.Click += new System.EventHandler(this.BtnGuardar_Click);

            this.btnCancelar.Location = new System.Drawing.Point(828, 696);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(100, 38);
            this.btnCancelar.TabIndex = 23;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);

            // ------------------------------- Formulario --------------------------------
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(944, 750);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.gbDetalle);
            this.Controls.Add(this.gbCabecera);
            this.Controls.Add(this.pnlTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAtencionEdicion";
            this.Text = "Atención";

            Hospital.Presentacion.Tema.AplicarFormulario(this);
            Hospital.Presentacion.Tema.Grupo(this.gbCabecera);
            Hospital.Presentacion.Tema.Grupo(this.gbDetalle);
            Hospital.Presentacion.Tema.Grilla(this.dgvDetalle);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnGuardar);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnCancelar);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnBuscarPaciente);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnBuscarCie);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnAgregarDetalle);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnQuitarDetalle);

            this.txtDocumento.BackColor = Hospital.Presentacion.Tema.GrisSuave;

            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.pnlTitulo.ResumeLayout(false);
            this.gbCabecera.ResumeLayout(false);
            this.gbCabecera.PerformLayout();
            this.gbDetalle.ResumeLayout(false);
            this.gbDetalle.PerformLayout();
            this.ResumeLayout(false);
        }

        private static System.Windows.Forms.DataGridViewTextBoxColumn ColumnaDetalle(string propiedad, string titulo, int peso)
        {
            return new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo,
                FillWeight = peso,
                ReadOnly = true
            };
        }
    }
}
