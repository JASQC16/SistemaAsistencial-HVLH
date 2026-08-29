namespace Hospital.Presentacion.Formularios
{
    partial class FrmPrincipal
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.MenuStrip menuPrincipal;
        private System.Windows.Forms.ToolStripMenuItem mnuRegistro;
        private System.Windows.Forms.ToolStripMenuItem mnuPacientes;
        private System.Windows.Forms.ToolStripMenuItem mnuCitas;
        private System.Windows.Forms.ToolStripMenuItem mnuAtenciones;
        private System.Windows.Forms.ToolStripMenuItem mnuReportes;
        private System.Windows.Forms.ToolStripMenuItem mnuReporteGeneral;
        private System.Windows.Forms.ToolStripMenuItem mnuSistema;
        private System.Windows.Forms.ToolStripMenuItem mnuCerrarSesion;
        private System.Windows.Forms.ToolStripSeparator sepSistema;
        private System.Windows.Forms.ToolStripMenuItem mnuSalir;
        private System.Windows.Forms.ToolStripMenuItem mnuAyuda;
        private System.Windows.Forms.ToolStripMenuItem mnuAcerca;

        private System.Windows.Forms.ToolStrip barraAccesos;
        private System.Windows.Forms.ToolStripButton btnPacientes;
        private System.Windows.Forms.ToolStripButton btnCitas;
        private System.Windows.Forms.ToolStripButton btnAtenciones;
        private System.Windows.Forms.ToolStripButton btnReportes;

        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblInstitucion;
        private System.Windows.Forms.Label lblModulo;

        private System.Windows.Forms.StatusStrip barraEstado;
        private System.Windows.Forms.ToolStripStatusLabel lblUsuario;
        private System.Windows.Forms.ToolStripStatusLabel lblSeparador;
        private System.Windows.Forms.ToolStripStatusLabel lblFecha;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.menuPrincipal = new System.Windows.Forms.MenuStrip();
            this.mnuRegistro = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPacientes = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCitas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAtenciones = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReportes = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReporteGeneral = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSistema = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCerrarSesion = new System.Windows.Forms.ToolStripMenuItem();
            this.sepSistema = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSalir = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAyuda = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAcerca = new System.Windows.Forms.ToolStripMenuItem();
            this.barraAccesos = new System.Windows.Forms.ToolStrip();
            this.btnPacientes = new System.Windows.Forms.ToolStripButton();
            this.btnCitas = new System.Windows.Forms.ToolStripButton();
            this.btnAtenciones = new System.Windows.Forms.ToolStripButton();
            this.btnReportes = new System.Windows.Forms.ToolStripButton();
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblInstitucion = new System.Windows.Forms.Label();
            this.lblModulo = new System.Windows.Forms.Label();
            this.barraEstado = new System.Windows.Forms.StatusStrip();
            this.lblUsuario = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSeparador = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblFecha = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuPrincipal.SuspendLayout();
            this.barraAccesos.SuspendLayout();
            this.pnlEncabezado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.barraEstado.SuspendLayout();
            this.SuspendLayout();

            // ---------------------------------------------------------------
            // menuPrincipal
            // ---------------------------------------------------------------
            this.menuPrincipal.BackColor = Hospital.Presentacion.Tema.GrisSuave;
            this.menuPrincipal.Font = Hospital.Presentacion.Tema.FuenteEtiqueta;
            this.menuPrincipal.ForeColor = Hospital.Presentacion.Tema.Texto;
            this.menuPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuRegistro, this.mnuReportes, this.mnuSistema, this.mnuAyuda});
            this.menuPrincipal.Location = new System.Drawing.Point(0, 0);
            this.menuPrincipal.Name = "menuPrincipal";
            this.menuPrincipal.Padding = new System.Windows.Forms.Padding(8, 4, 0, 4);
            this.menuPrincipal.Size = new System.Drawing.Size(1180, 30);

            // mnuRegistro
            this.mnuRegistro.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuPacientes, this.mnuCitas, this.mnuAtenciones});
            this.mnuRegistro.Name = "mnuRegistro";
            this.mnuRegistro.Text = "Registro asistencial";

            this.mnuPacientes.Name = "mnuPacientes";
            this.mnuPacientes.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P;
            this.mnuPacientes.Text = "PACIENTES";
            this.mnuPacientes.Click += new System.EventHandler(this.MnuPacientes_Click);

            this.mnuCitas.Name = "mnuCitas";
            this.mnuCitas.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T;
            this.mnuCitas.Text = "CITAS";
            this.mnuCitas.Click += new System.EventHandler(this.MnuCitas_Click);

            this.mnuAtenciones.Name = "mnuAtenciones";
            this.mnuAtenciones.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A;
            this.mnuAtenciones.Text = "ATENCIONES";
            this.mnuAtenciones.Click += new System.EventHandler(this.MnuAtenciones_Click);

            // mnuReportes
            this.mnuReportes.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuReporteGeneral});
            this.mnuReportes.Name = "mnuReportes";
            this.mnuReportes.Text = "Reportes";

            this.mnuReporteGeneral.Name = "mnuReporteGeneral";
            this.mnuReporteGeneral.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R;
            this.mnuReporteGeneral.Text = "Reporte de pacientes y atenciones";
            this.mnuReporteGeneral.Click += new System.EventHandler(this.MnuReportes_Click);

            // mnuSistema
            this.mnuSistema.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuCerrarSesion, this.sepSistema, this.mnuSalir});
            this.mnuSistema.Name = "mnuSistema";
            this.mnuSistema.Text = "Sistema";

            this.mnuCerrarSesion.Name = "mnuCerrarSesion";
            this.mnuCerrarSesion.Text = "Cerrar sesión";
            this.mnuCerrarSesion.Click += new System.EventHandler(this.MnuCerrarSesion_Click);

            this.sepSistema.Name = "sepSistema";

            this.mnuSalir.Name = "mnuSalir";
            this.mnuSalir.Text = "Salir";
            this.mnuSalir.Click += new System.EventHandler(this.MnuSalir_Click);

            // mnuAyuda
            this.mnuAyuda.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuAcerca });
            this.mnuAyuda.Name = "mnuAyuda";
            this.mnuAyuda.Text = "Ayuda";

            this.mnuAcerca.Name = "mnuAcerca";
            this.mnuAcerca.Text = "Acerca de";
            this.mnuAcerca.Click += new System.EventHandler(this.MnuAcerca_Click);

            // ---------------------------------------------------------------
            // pnlEncabezado: identidad institucional siempre visible
            // ---------------------------------------------------------------
            this.pnlEncabezado.BackColor = Hospital.Presentacion.Tema.Blanco;
            this.pnlEncabezado.Controls.Add(this.picLogo);
            this.pnlEncabezado.Controls.Add(this.lblInstitucion);
            this.pnlEncabezado.Controls.Add(this.lblModulo);
            this.pnlEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEncabezado.Location = new System.Drawing.Point(0, 30);
            this.pnlEncabezado.Name = "pnlEncabezado";
            this.pnlEncabezado.Size = new System.Drawing.Size(1180, 72);
            this.pnlEncabezado.Paint += new System.Windows.Forms.PaintEventHandler(this.PnlEncabezado_Paint);

            this.picLogo.BackColor = System.Drawing.Color.Transparent;
            this.picLogo.Location = new System.Drawing.Point(16, 6);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(58, 58);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabStop = false;

            this.lblInstitucion.AutoSize = false;
            this.lblInstitucion.Font = new System.Drawing.Font("Segoe UI Semibold", 13F);
            this.lblInstitucion.ForeColor = Hospital.Presentacion.Tema.AzulProfundo;
            this.lblInstitucion.Location = new System.Drawing.Point(86, 12);
            this.lblInstitucion.Name = "lblInstitucion";
            this.lblInstitucion.Size = new System.Drawing.Size(760, 26);
            this.lblInstitucion.Text = "Hospital Nacional Víctor Larco Herrera - HVLH";

            this.lblModulo.AutoSize = false;
            this.lblModulo.Font = Hospital.Presentacion.Tema.FuenteSubtitulo;
            this.lblModulo.ForeColor = Hospital.Presentacion.Tema.TextoSuave;
            this.lblModulo.Location = new System.Drawing.Point(88, 40);
            this.lblModulo.Name = "lblModulo";
            this.lblModulo.Size = new System.Drawing.Size(760, 20);
            this.lblModulo.Text = "Sistema de atenciones ambulatorias";

            // ---------------------------------------------------------------
            // barraAccesos: los cuatro módulos, siempre a la vista
            // ---------------------------------------------------------------
            this.barraAccesos.BackColor = Hospital.Presentacion.Tema.GrisSuave;
            this.barraAccesos.Font = Hospital.Presentacion.Tema.FuenteEtiqueta;
            this.barraAccesos.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.barraAccesos.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnPacientes, this.btnCitas, this.btnAtenciones, this.btnReportes});
            this.barraAccesos.Location = new System.Drawing.Point(0, 102);
            this.barraAccesos.Name = "barraAccesos";
            this.barraAccesos.Padding = new System.Windows.Forms.Padding(10, 4, 0, 4);
            this.barraAccesos.Size = new System.Drawing.Size(1180, 34);

            this.btnPacientes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPacientes.ForeColor = Hospital.Presentacion.Tema.AzulProfundo;
            this.btnPacientes.Name = "btnPacientes";
            this.btnPacientes.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnPacientes.Text = "PACIENTES";
            this.btnPacientes.ToolTipText = "Registro y consulta de pacientes (Ctrl+P)";
            this.btnPacientes.Click += new System.EventHandler(this.MnuPacientes_Click);

            this.btnCitas.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCitas.ForeColor = Hospital.Presentacion.Tema.AzulProfundo;
            this.btnCitas.Name = "btnCitas";
            this.btnCitas.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnCitas.Text = "CITAS";
            this.btnCitas.ToolTipText = "Agenda de citas (Ctrl+T)";
            this.btnCitas.Click += new System.EventHandler(this.MnuCitas_Click);

            this.btnAtenciones.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAtenciones.ForeColor = Hospital.Presentacion.Tema.AzulProfundo;
            this.btnAtenciones.Name = "btnAtenciones";
            this.btnAtenciones.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnAtenciones.Text = "ATENCIONES";
            this.btnAtenciones.ToolTipText = "Registro de atenciones ambulatorias (Ctrl+A)";
            this.btnAtenciones.Click += new System.EventHandler(this.MnuAtenciones_Click);

            this.btnReportes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnReportes.ForeColor = Hospital.Presentacion.Tema.AzulProfundo;
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnReportes.Text = "REPORTES";
            this.btnReportes.ToolTipText = "Reporte de pacientes y atenciones (Ctrl+R)";
            this.btnReportes.Click += new System.EventHandler(this.MnuReportes_Click);

            // ---------------------------------------------------------------
            // barraEstado
            // ---------------------------------------------------------------
            this.barraEstado.BackColor = Hospital.Presentacion.Tema.GrisSuave;
            this.barraEstado.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblUsuario, this.lblSeparador, this.lblFecha});
            this.barraEstado.Location = new System.Drawing.Point(0, 618);
            this.barraEstado.Name = "barraEstado";
            this.barraEstado.Size = new System.Drawing.Size(1180, 24);

            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.ForeColor = Hospital.Presentacion.Tema.Texto;
            this.lblUsuario.Text = "Usuario";

            this.lblSeparador.Name = "lblSeparador";
            this.lblSeparador.Spring = true;
            this.lblSeparador.Text = "";

            this.lblFecha.Name = "lblFecha";
            this.lblFecha.ForeColor = Hospital.Presentacion.Tema.TextoSuave;
            this.lblFecha.Text = "Fecha";

            // ---------------------------------------------------------------
            // FrmPrincipal
            // ---------------------------------------------------------------
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1180, 642);
            this.Controls.Add(this.barraEstado);
            this.Controls.Add(this.barraAccesos);
            this.Controls.Add(this.pnlEncabezado);
            this.Controls.Add(this.menuPrincipal);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuPrincipal;
            this.Name = "FrmPrincipal";
            this.Text = "Hospital Nacional Víctor Larco Herrera - HVLH";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            Hospital.Presentacion.Tema.AplicarFormulario(this);
            this.BackColor = Hospital.Presentacion.Tema.Gris;

            this.menuPrincipal.ResumeLayout(false);
            this.barraAccesos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlEncabezado.ResumeLayout(false);
            this.barraEstado.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
