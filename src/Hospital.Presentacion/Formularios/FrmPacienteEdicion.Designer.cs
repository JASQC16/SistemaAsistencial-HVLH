namespace Hospital.Presentacion.Formularios
{
    partial class FrmPacienteEdicion
    {
        private System.ComponentModel.IContainer components = null;

        private Hospital.Presentacion.Controles.EncabezadoHvlh encabezado;

        private System.Windows.Forms.GroupBox grpIdentificacion;
        private System.Windows.Forms.Label lblTipoDocumento;
        private System.Windows.Forms.ComboBox cboTipoDocumento;
        private System.Windows.Forms.Label lblNumeroDocumento;
        private System.Windows.Forms.TextBox txtNumeroDocumento;
        private System.Windows.Forms.Label lblHistoriaClinica;
        private System.Windows.Forms.TextBox txtHistoriaClinica;
        private System.Windows.Forms.Label lblApellidoPaterno;
        private System.Windows.Forms.TextBox txtApellidoPaterno;
        private System.Windows.Forms.Label lblApellidoMaterno;
        private System.Windows.Forms.TextBox txtApellidoMaterno;
        private System.Windows.Forms.Label lblNombres;
        private System.Windows.Forms.TextBox txtNombres;
        private System.Windows.Forms.Label lblFechaNacimiento;
        private System.Windows.Forms.DateTimePicker dtpFechaNacimiento;
        private System.Windows.Forms.Label lblEdad;
        private System.Windows.Forms.TextBox txtEdad;
        private System.Windows.Forms.Label lblSexo;
        private System.Windows.Forms.ComboBox cboSexo;

        private System.Windows.Forms.GroupBox grpContacto;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label lblCorreo;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.TextBox txtDireccion;

        private System.Windows.Forms.Label lblEstadoRegistro;
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
            this.grpIdentificacion = new System.Windows.Forms.GroupBox();
            this.lblTipoDocumento = new System.Windows.Forms.Label();
            this.cboTipoDocumento = new System.Windows.Forms.ComboBox();
            this.lblNumeroDocumento = new System.Windows.Forms.Label();
            this.txtNumeroDocumento = new System.Windows.Forms.TextBox();
            this.lblHistoriaClinica = new System.Windows.Forms.Label();
            this.txtHistoriaClinica = new System.Windows.Forms.TextBox();
            this.lblApellidoPaterno = new System.Windows.Forms.Label();
            this.txtApellidoPaterno = new System.Windows.Forms.TextBox();
            this.lblApellidoMaterno = new System.Windows.Forms.Label();
            this.txtApellidoMaterno = new System.Windows.Forms.TextBox();
            this.lblNombres = new System.Windows.Forms.Label();
            this.txtNombres = new System.Windows.Forms.TextBox();
            this.lblFechaNacimiento = new System.Windows.Forms.Label();
            this.dtpFechaNacimiento = new System.Windows.Forms.DateTimePicker();
            this.lblEdad = new System.Windows.Forms.Label();
            this.txtEdad = new System.Windows.Forms.TextBox();
            this.lblSexo = new System.Windows.Forms.Label();
            this.cboSexo = new System.Windows.Forms.ComboBox();
            this.grpContacto = new System.Windows.Forms.GroupBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblCorreo = new System.Windows.Forms.Label();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.lblEstadoRegistro = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.grpIdentificacion.SuspendLayout();
            this.grpContacto.SuspendLayout();
            this.SuspendLayout();

            // encabezado
            this.encabezado.Name = "encabezado";
            this.encabezado.Titulo = "Registro de paciente";

            // ---------------------------------------------------------------
            // grpIdentificacion
            // ---------------------------------------------------------------
            this.grpIdentificacion.Controls.Add(this.lblTipoDocumento);
            this.grpIdentificacion.Controls.Add(this.cboTipoDocumento);
            this.grpIdentificacion.Controls.Add(this.lblNumeroDocumento);
            this.grpIdentificacion.Controls.Add(this.txtNumeroDocumento);
            this.grpIdentificacion.Controls.Add(this.lblHistoriaClinica);
            this.grpIdentificacion.Controls.Add(this.txtHistoriaClinica);
            this.grpIdentificacion.Controls.Add(this.lblApellidoPaterno);
            this.grpIdentificacion.Controls.Add(this.txtApellidoPaterno);
            this.grpIdentificacion.Controls.Add(this.lblApellidoMaterno);
            this.grpIdentificacion.Controls.Add(this.txtApellidoMaterno);
            this.grpIdentificacion.Controls.Add(this.lblNombres);
            this.grpIdentificacion.Controls.Add(this.txtNombres);
            this.grpIdentificacion.Controls.Add(this.lblFechaNacimiento);
            this.grpIdentificacion.Controls.Add(this.dtpFechaNacimiento);
            this.grpIdentificacion.Controls.Add(this.lblEdad);
            this.grpIdentificacion.Controls.Add(this.txtEdad);
            this.grpIdentificacion.Controls.Add(this.lblSexo);
            this.grpIdentificacion.Controls.Add(this.cboSexo);
            this.grpIdentificacion.Location = new System.Drawing.Point(18, 86);
            this.grpIdentificacion.Name = "grpIdentificacion";
            this.grpIdentificacion.Size = new System.Drawing.Size(664, 240);
            this.grpIdentificacion.TabStop = false;
            this.grpIdentificacion.Text = "Identificación";

            this.lblTipoDocumento.AutoSize = true;
            this.lblTipoDocumento.Location = new System.Drawing.Point(18, 30);
            this.lblTipoDocumento.Text = "Tipo de documento *";

            this.cboTipoDocumento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoDocumento.Location = new System.Drawing.Point(18, 48);
            this.cboTipoDocumento.Name = "cboTipoDocumento";
            this.cboTipoDocumento.Size = new System.Drawing.Size(210, 25);
            this.cboTipoDocumento.SelectedIndexChanged += new System.EventHandler(this.CboTipoDocumento_SelectedIndexChanged);

            this.lblNumeroDocumento.AutoSize = true;
            this.lblNumeroDocumento.Location = new System.Drawing.Point(244, 30);
            this.lblNumeroDocumento.Text = "Número de documento *";

            this.txtNumeroDocumento.Location = new System.Drawing.Point(244, 48);
            this.txtNumeroDocumento.MaxLength = 15;
            this.txtNumeroDocumento.Name = "txtNumeroDocumento";
            this.txtNumeroDocumento.Size = new System.Drawing.Size(190, 25);
            this.txtNumeroDocumento.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNumeroDocumento.Leave += new System.EventHandler(this.TxtNumeroDocumento_Leave);

            this.lblHistoriaClinica.AutoSize = true;
            this.lblHistoriaClinica.Location = new System.Drawing.Point(450, 30);
            this.lblHistoriaClinica.Text = "N.º de historia clínica";

            this.txtHistoriaClinica.Location = new System.Drawing.Point(450, 48);
            this.txtHistoriaClinica.Name = "txtHistoriaClinica";
            this.txtHistoriaClinica.ReadOnly = true;
            this.txtHistoriaClinica.Size = new System.Drawing.Size(190, 25);
            this.txtHistoriaClinica.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHistoriaClinica.TabStop = false;

            this.lblApellidoPaterno.AutoSize = true;
            this.lblApellidoPaterno.Location = new System.Drawing.Point(18, 88);
            this.lblApellidoPaterno.Text = "Apellido paterno *";

            this.txtApellidoPaterno.Location = new System.Drawing.Point(18, 106);
            this.txtApellidoPaterno.MaxLength = 40;
            this.txtApellidoPaterno.Name = "txtApellidoPaterno";
            this.txtApellidoPaterno.Size = new System.Drawing.Size(210, 25);
            this.txtApellidoPaterno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblApellidoMaterno.AutoSize = true;
            this.lblApellidoMaterno.Location = new System.Drawing.Point(244, 88);
            this.lblApellidoMaterno.Text = "Apellido materno";

            this.txtApellidoMaterno.Location = new System.Drawing.Point(244, 106);
            this.txtApellidoMaterno.MaxLength = 40;
            this.txtApellidoMaterno.Name = "txtApellidoMaterno";
            this.txtApellidoMaterno.Size = new System.Drawing.Size(190, 25);
            this.txtApellidoMaterno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblNombres.AutoSize = true;
            this.lblNombres.Location = new System.Drawing.Point(450, 88);
            this.lblNombres.Text = "Nombres *";

            this.txtNombres.Location = new System.Drawing.Point(450, 106);
            this.txtNombres.MaxLength = 60;
            this.txtNombres.Name = "txtNombres";
            this.txtNombres.Size = new System.Drawing.Size(190, 25);
            this.txtNombres.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblFechaNacimiento.AutoSize = true;
            this.lblFechaNacimiento.Location = new System.Drawing.Point(18, 146);
            this.lblFechaNacimiento.Text = "Fecha de nacimiento *";

            this.dtpFechaNacimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaNacimiento.Location = new System.Drawing.Point(18, 164);
            this.dtpFechaNacimiento.Name = "dtpFechaNacimiento";
            this.dtpFechaNacimiento.Size = new System.Drawing.Size(210, 25);
            this.dtpFechaNacimiento.ValueChanged += new System.EventHandler(this.DtpFechaNacimiento_ValueChanged);

            this.lblEdad.AutoSize = true;
            this.lblEdad.Location = new System.Drawing.Point(244, 146);
            this.lblEdad.Text = "Edad";

            this.txtEdad.Location = new System.Drawing.Point(244, 164);
            this.txtEdad.Name = "txtEdad";
            this.txtEdad.ReadOnly = true;
            this.txtEdad.Size = new System.Drawing.Size(190, 25);
            this.txtEdad.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEdad.TabStop = false;

            this.lblSexo.AutoSize = true;
            this.lblSexo.Location = new System.Drawing.Point(450, 146);
            this.lblSexo.Text = "Sexo *";

            this.cboSexo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSexo.Location = new System.Drawing.Point(450, 164);
            this.cboSexo.Name = "cboSexo";
            this.cboSexo.Size = new System.Drawing.Size(190, 25);

            // ---------------------------------------------------------------
            // grpContacto
            // ---------------------------------------------------------------
            this.grpContacto.Controls.Add(this.lblTelefono);
            this.grpContacto.Controls.Add(this.txtTelefono);
            this.grpContacto.Controls.Add(this.lblCorreo);
            this.grpContacto.Controls.Add(this.txtCorreo);
            this.grpContacto.Controls.Add(this.lblDireccion);
            this.grpContacto.Controls.Add(this.txtDireccion);
            this.grpContacto.Location = new System.Drawing.Point(18, 336);
            this.grpContacto.Name = "grpContacto";
            this.grpContacto.Size = new System.Drawing.Size(664, 148);
            this.grpContacto.TabStop = false;
            this.grpContacto.Text = "Contacto";

            this.lblTelefono.AutoSize = true;
            this.lblTelefono.Location = new System.Drawing.Point(18, 30);
            this.lblTelefono.Text = "Teléfono";

            this.txtTelefono.Location = new System.Drawing.Point(18, 48);
            this.txtTelefono.MaxLength = 20;
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(210, 25);
            this.txtTelefono.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblCorreo.AutoSize = true;
            this.lblCorreo.Location = new System.Drawing.Point(244, 30);
            this.lblCorreo.Text = "Correo electrónico";

            this.txtCorreo.Location = new System.Drawing.Point(244, 48);
            this.txtCorreo.MaxLength = 100;
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.Size = new System.Drawing.Size(396, 25);
            this.txtCorreo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblDireccion.AutoSize = true;
            this.lblDireccion.Location = new System.Drawing.Point(18, 82);
            this.lblDireccion.Text = "Dirección";

            this.txtDireccion.Location = new System.Drawing.Point(18, 100);
            this.txtDireccion.MaxLength = 150;
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(622, 25);
            this.txtDireccion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // ---------------------------------------------------------------
            // Pie
            // ---------------------------------------------------------------
            this.lblEstadoRegistro.AutoSize = false;
            this.lblEstadoRegistro.Location = new System.Drawing.Point(18, 498);
            this.lblEstadoRegistro.Name = "lblEstadoRegistro";
            this.lblEstadoRegistro.Size = new System.Drawing.Size(390, 36);
            this.lblEstadoRegistro.Text = "";

            this.btnGuardar.Location = new System.Drawing.Point(432, 496);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(130, 36);
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.BtnGuardar_Click);

            this.btnCancelar.Location = new System.Drawing.Point(572, 496);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(110, 36);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);

            // ---------------------------------------------------------------
            // FrmPacienteEdicion
            // ---------------------------------------------------------------
            this.AcceptButton = this.btnGuardar;
            this.CancelButton = this.btnCancelar;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(700, 548);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.lblEstadoRegistro);
            this.Controls.Add(this.grpContacto);
            this.Controls.Add(this.grpIdentificacion);
            this.Controls.Add(this.encabezado);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmPacienteEdicion";
            this.Text = "HVLH — Paciente";
            this.Load += new System.EventHandler(this.FrmPacienteEdicion_Load);

            Hospital.Presentacion.Tema.AplicarFormulario(this);
            Hospital.Presentacion.Tema.Grupo(this.grpIdentificacion);
            Hospital.Presentacion.Tema.Grupo(this.grpContacto);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnGuardar);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnCancelar);
            this.lblEstadoRegistro.ForeColor = Hospital.Presentacion.Tema.TextoSuave;

            this.grpIdentificacion.ResumeLayout(false);
            this.grpIdentificacion.PerformLayout();
            this.grpContacto.ResumeLayout(false);
            this.grpContacto.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
