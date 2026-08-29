namespace Hospital.Presentacion.Formularios
{
    partial class FrmLogin
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblInstitucion;
        private System.Windows.Forms.Label lblSistema;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblClave;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Button btnIngresar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Label lblAyuda;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblInstitucion = new System.Windows.Forms.Label();
            this.lblSistema = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblClave = new System.Windows.Forms.Label();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.btnIngresar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.lblAyuda = new System.Windows.Forms.Label();
            this.pnlEncabezado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();

            // pnlEncabezado
            this.pnlEncabezado.BackColor = Hospital.Presentacion.Tema.Blanco;
            this.pnlEncabezado.Controls.Add(this.picLogo);
            this.pnlEncabezado.Controls.Add(this.lblInstitucion);
            this.pnlEncabezado.Controls.Add(this.lblSistema);
            this.pnlEncabezado.Paint += new System.Windows.Forms.PaintEventHandler(this.PnlEncabezado_Paint);
            this.pnlEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEncabezado.Location = new System.Drawing.Point(0, 0);
            this.pnlEncabezado.Name = "pnlEncabezado";
            this.pnlEncabezado.Size = new System.Drawing.Size(420, 104);
            this.pnlEncabezado.TabIndex = 0;

            // picLogo: logo oficial del Hospital Nacional Víctor Larco Herrera
            this.picLogo.BackColor = System.Drawing.Color.Transparent;
            this.picLogo.Location = new System.Drawing.Point(24, 16);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(72, 72);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 99;
            this.picLogo.TabStop = false;

            // lblInstitucion
            this.lblInstitucion.AutoSize = false;
            this.lblInstitucion.Font = Hospital.Presentacion.Tema.FuenteTitulo;
            this.lblInstitucion.ForeColor = Hospital.Presentacion.Tema.AzulProfundo;
            this.lblInstitucion.Location = new System.Drawing.Point(106, 24);
            this.lblInstitucion.Name = "lblInstitucion";
            this.lblInstitucion.Size = new System.Drawing.Size(292, 44);
            this.lblInstitucion.Text = "Hospital Nacional Víctor Larco Herrera";

            // lblSistema
            this.lblSistema.AutoSize = false;
            this.lblSistema.Font = Hospital.Presentacion.Tema.FuenteSubtitulo;
            this.lblSistema.ForeColor = Hospital.Presentacion.Tema.TextoSuave;
            this.lblSistema.Location = new System.Drawing.Point(108, 66);
            this.lblSistema.Name = "lblSistema";
            this.lblSistema.Size = new System.Drawing.Size(292, 20);
            this.lblSistema.Text = "HVLH — Sistema de atenciones ambulatorias";

            // lblUsuario
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = Hospital.Presentacion.Tema.FuenteEtiqueta;
            this.lblUsuario.Location = new System.Drawing.Point(46, 128);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(50, 17);
            this.lblUsuario.Text = "Usuario";

            // txtUsuario
            this.txtUsuario.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsuario.Location = new System.Drawing.Point(46, 150);
            this.txtUsuario.MaxLength = 30;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(328, 25);
            this.txtUsuario.TabIndex = 1;
            this.txtUsuario.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Campo_KeyDown);

            // lblClave
            this.lblClave.AutoSize = true;
            this.lblClave.Font = Hospital.Presentacion.Tema.FuenteEtiqueta;
            this.lblClave.Location = new System.Drawing.Point(46, 188);
            this.lblClave.Name = "lblClave";
            this.lblClave.Size = new System.Drawing.Size(70, 17);
            this.lblClave.Text = "Contraseña";

            // txtClave
            this.txtClave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtClave.Location = new System.Drawing.Point(46, 210);
            this.txtClave.MaxLength = 50;
            this.txtClave.Name = "txtClave";
            this.txtClave.Size = new System.Drawing.Size(328, 25);
            this.txtClave.TabIndex = 2;
            this.txtClave.UseSystemPasswordChar = true;
            this.txtClave.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Campo_KeyDown);

            // lblMensaje
            this.lblMensaje.AutoSize = false;
            this.lblMensaje.ForeColor = Hospital.Presentacion.Tema.Rojo;
            this.lblMensaje.Location = new System.Drawing.Point(46, 243);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(328, 34);
            this.lblMensaje.Text = "";

            // btnIngresar
            this.btnIngresar.Location = new System.Drawing.Point(46, 284);
            this.btnIngresar.Name = "btnIngresar";
            this.btnIngresar.Size = new System.Drawing.Size(206, 38);
            this.btnIngresar.TabIndex = 3;
            this.btnIngresar.Text = "Ingresar";
            this.btnIngresar.Click += new System.EventHandler(this.BtnIngresar_Click);

            // btnSalir
            this.btnSalir.Location = new System.Drawing.Point(262, 284);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(112, 38);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.BtnSalir_Click);

            // lblAyuda
            this.lblAyuda.AutoSize = false;
            this.lblAyuda.ForeColor = Hospital.Presentacion.Tema.TextoSuave;
            this.lblAyuda.Location = new System.Drawing.Point(46, 330);
            this.lblAyuda.Name = "lblAyuda";
            this.lblAyuda.Size = new System.Drawing.Size(328, 20);
            this.lblAyuda.Text = "Usuario de prueba: admin  /  Admin123$";

            // FrmLogin
            this.AcceptButton = this.btnIngresar;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(420, 366);
            this.Controls.Add(this.lblAyuda);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.btnIngresar);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.txtClave);
            this.Controls.Add(this.lblClave);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.pnlEncabezado);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLogin";
            this.Text = "HVLH — Acceso al sistema";

            Hospital.Presentacion.Tema.AplicarFormulario(this);
            Hospital.Presentacion.Tema.CampoTexto(this.txtUsuario);
            Hospital.Presentacion.Tema.CampoTexto(this.txtClave);
            Hospital.Presentacion.Tema.BotonPrimario(this.btnIngresar);
            Hospital.Presentacion.Tema.BotonSecundario(this.btnSalir);

            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlEncabezado.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
