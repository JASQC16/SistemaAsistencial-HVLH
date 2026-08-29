namespace Hospital.Presentacion.Formularios
{
    partial class FrmCitaEdicion
    {
        private System.ComponentModel.IContainer components = null;

        private Hospital.Presentacion.Controles.EncabezadoHvlh encabezado;

        private System.Windows.Forms.GroupBox grpDatos;
        private System.Windows.Forms.Label lblPaciente;
        private System.Windows.Forms.TextBox txtPaciente;
        private System.Windows.Forms.Button btnBuscarPaciente;
        private System.Windows.Forms.Button btnNuevoPaciente;
        private System.Windows.Forms.Label lblDatosPaciente;
        private System.Windows.Forms.Label lblMedico;
        private System.Windows.Forms.ComboBox cboMedico;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblHora;
        private System.Windows.Forms.DateTimePicker dtpHora;
        private System.Windows.Forms.Label lblMotivo;
        private System.Windows.Forms.TextBox txtMotivo;
        private System.Windows.Forms.Label lblObservaciones;
        private System.Windows.Forms.TextBox txtObservaciones;

        private System.Windows.Forms.Label lblEstadoCita;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.encabezado = new Hospital.Presentacion.Controles.EncabezadoHvlh();
            this.grpDatos = new System.Windows.Forms.GroupBox();
            this.lblPaciente = new System.Windows.Forms.Label();
            this.txtPaciente = new System.Windows.Forms.TextBox();
            this.btnBuscarPaciente = new System.Windows.Forms.Button();
            this.btnNuevoPaciente = new System.Windows.Forms.Button();
            this.lblDatosPaciente = new System.Windows.Forms.Label();
            this.lblMedico = new System.Windows.Forms.Label();
            this.cboMedico = new System.Windows.Forms.ComboBox();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lblHora = new System.Windows.Forms.Label();
            this.dtpHora = new System.Windows.Forms.DateTimePicker();
            this.lblMotivo = new System.Windows.Forms.Label();
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.lblObservaciones = new System.Windows.Forms.Label();
            this.txtObservaciones = new System.Windows.Forms.TextBox();
            this.lblEstadoCita = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.grpDatos.SuspendLayout();
            this.SuspendLayout();

            // encabezado
            this.encabezado.Name = "encabezado";
            this.encabezado.Titulo = "Programación de cita";

            // ---------------------------------------------------------------
            // grpDatos
            // ---------------------------------------------------------------
            this.grpDatos.Controls.Add(this.lblPaciente);
            this.grpDatos.Controls.Add(this.txtPaciente);
            this.grpDatos.Controls.Add(this.btnBuscarPaciente);
            this.grpDatos.Controls.Add(this.btnNuevoPaciente);
            this.grpDatos.Controls.Add(this.lblDatosPaciente);
            this.grpDatos.Controls.Add(this.lblMedico);
            this.grpDatos.Controls.Add(this.cboMedico);
            this.grpDatos.Controls.Add(this.lblFecha);
            this.grpDatos.Controls.Add(this.dtpFecha);
            this.grpDatos.Controls.Add(this.lblHora);
            this.grpDatos.Controls.Add(this.dtpHora);
            this.grpDatos.Controls.Add(this.lblMotivo);
            this.grpDatos.Controls.Add(this.txtMotivo);
            this.grpDatos.Controls.Add(this.lblObservaciones);
            this.grpDatos.Controls.Add(this.txtObservaciones);
            this.grpDatos.Location = new System.Drawing.Point(18, 86);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(664, 366);
            this.grpDatos.TabStop = false;
            this.grpDatos.Text = "Datos de la cita";

            this.lblPaciente.AutoSize = true;
            this.lblPaciente.Location = new System.Drawing.Point(18, 30);
            this.lblPaciente.Text = "Paciente *";

            this.txtPaciente.Location = new System.Drawing.Point(18, 48);
            this.txtPaciente.Name = "txtPaciente";
            this.txtPaciente.ReadOnly = true;
            this.txtPaciente.Size = new System.Drawing.Size(400, 25);
            this.txtPaciente.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPaciente.TabStop = false;

            this.btnBuscarPaciente.Location = new System.Drawing.Point(426, 47);
            this.btnBuscarPaciente.Name = "btnBuscarPaciente";
            this.btnBuscarPaciente.Size = new System.Drawing.Size(100, 27);
            this.btnBuscarPaciente.Text = "Buscar...";
            this.btnBuscarPaciente.Click += new System.EventHandler(this.BtnBuscarPaciente_Click);

            this.btnNuevoPaciente.Location = new System.Drawing.Point(534, 47);
            this.btnNuevoPaciente.Name = "btnNuevoPaciente";
            this.btnNuevoPaciente.Size = new System.Drawing.Size(106, 27);
            this.btnNuevoPaciente.Text = "Registrar";
            this.btnNuevoPaciente.Click += new System.EventHandler(this.BtnNuevoPaciente_Click);

            this.lblDatosPaciente.AutoSize = false;
            this.lblDatosPaciente.Location = new System.Drawing.Point(18, 78);
            this.lblDatosPaciente.Name = "lblDatosPaciente";
            this.lblDatosPaciente.Size = new System.Drawing.Size(622, 20);
            this.lblDatosPaciente.Text = "";

            this.lblMedico.AutoSize = true;
            this.lblMedico.Location = new System.Drawing.Point(18, 110);
            this.lblMedico.Text = "Profesional *";

            this.cboMedico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMedico.Location = new System.Drawing.Point(18, 128);
            this.cboMedico.Name = "cboMedico";
            this.cboMedico.Size = new System.Drawing.Size(400, 25);

            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(18, 168);
            this.lblFecha.Text = "Fecha de la cita *";

            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(18, 186);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(190, 25);

            this.lblHora.AutoSize = true;
            this.lblHora.Location = new System.Drawing.Point(228, 168);
            this.lblHora.Text = "Hora *";

            this.dtpHora.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHora.Location = new System.Drawing.Point(228, 186);
            this.dtpHora.Name = "dtpHora";
            this.dtpHora.ShowUpDown = true;
            this.dtpHora.Size = new System.Drawing.Size(120, 25);

            this.lblMotivo.AutoSize = true;
            this.lblMotivo.Location = new System.Drawing.Point(18, 226);
            this.lblMotivo.Text = "Motivo de la cita";

            this.txtMotivo.Location = new System.Drawing.Point(18, 244);
            this.txtMotivo.MaxLength = 300;
            this.txtMotivo.Name = "txtMotivo";
            this.txtMotivo.Size = new System.Drawing.Size(622, 25);
            this.txtMotivo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblObservaciones.AutoSize = true;
            this.lblObservaciones.Location = new System.Drawing.Point(18, 280);
            this.lblObservaciones.Text = "Observaciones";

            this.txtObservaciones.Location = new System.Drawing.Point(18, 298);
            this.txtObservaciones.MaxLength = 500;
            this.txtObservaciones.Multiline = true;
            this.txtObservaciones.Name = "txtObservaciones";
            this.txtObservaciones.Size = new System.Drawing.Size(622, 52);
            this.txtObservaciones.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // ---------------------------------------------------------------
            // Pie
            // ---------------------------------------------------------------
            this.lblEstadoCita.AutoSize = false;
            this.lblEstadoCita.Location = new System.Drawing.Point(18, 466);
            this.lblEstadoCita.Name = "lblEstadoCita";
            this.lblEstadoCita.Size = new System.Drawing.Size(400, 36);
            this.lblEstadoCita.Text = "";

            this.btnGuardar.Location = new System.Drawing.Point(432, 464);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(130, 36);
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.BtnGuardar_Click);

            this.btnCancelar.Location = new System.Drawing.Point(572, 464);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(110, 36);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);

            // ---------------------------------------------------------------
            // FrmCitaEdicion
            // ---------------------------------------------------------------
            this.AcceptButton = this.btnGuardar;
            this.CancelButton = this.btnCancelar;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(700, 516);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.lblEstadoCita);
            this.Controls.Add(this.grpDatos);
            this.Controls.Add(this.encabezado);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmCitaEdicion";
            this.Text = "HVLH — Cita";
            this.Load += new System.EventHandler(this.FrmCitaEdicion_Load);

            Hospital.Presentacion.Tema.AplicarFormulario(this);
            Hospital.Presentacion.Tema.Grupo(this.grpDatos);
            Hospital.Presentacion.Tema.CampoTexto(this.txtPaciente);
            Hospital.Presentacion.Tema.CampoTexto(this.txtMotivo);
            Hospital.Presentacion.Tema.CampoTexto(this.txtObservaciones);
            Hospital.Presentacion.Tema.CampoTexto(this.cboMedico);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnBuscarPaciente);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnNuevoPaciente);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnGuardar);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnCancelar);
            this.lblDatosPaciente.ForeColor = Hospital.Presentacion.Tema.TextoSuave;
            this.lblEstadoCita.ForeColor = Hospital.Presentacion.Tema.TextoSuave;

            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
