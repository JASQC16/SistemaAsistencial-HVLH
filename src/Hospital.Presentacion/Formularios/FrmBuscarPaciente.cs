using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Hospital.Entidades;
using Hospital.Negocio;
using Hospital.Utilidades;

namespace Hospital.Presentacion.Formularios
{
    /// <summary>Selector de paciente por documento, nombres o apellidos.</summary>
    public class FrmBuscarPaciente : Form
    {
        private readonly MaestroServicio _servicio = new MaestroServicio();
        private readonly TextBox _txtBusqueda;
        private readonly DataGridView _grilla;
        private readonly Label _lblResumen;

        public Paciente PacienteSeleccionado { get; private set; }

        public FrmBuscarPaciente(string busquedaInicial)
        {
            var lblBuscar = new Label
            {
                Text = "Documento, nombres o apellidos",
                Location = new Point(18, 20),
                Size = new Size(200, 20),
                Font = Tema.FuenteEtiqueta
            };

            _txtBusqueda = new TextBox
            {
                Location = new Point(222, 17),
                Size = new Size(280, 25),
                MaxLength = 100,
                BorderStyle = BorderStyle.FixedSingle,
                Text = busquedaInicial ?? string.Empty
            };

            var btnBuscar = new Button { Text = "Buscar", Location = new Point(514, 16), Size = new Size(100, 28) };
            btnBuscar.Click += (s, e) => Buscar();

            _grilla = new DataGridView
            {
                Location = new Point(18, 56),
                Size = new Size(596, 300),
                AutoGenerateColumns = false,
                ReadOnly = true
            };
            _grilla.Columns.AddRange(
                NuevaColumna("NumeroDocumento", "Documento", 80),
                NuevaColumna("NombreCompleto", "Paciente", 220),
                NuevaColumna("Edad", "Edad", 50),
                NuevaColumna("Sexo", "Sexo", 50),
                NuevaColumna("Telefono", "Teléfono", 90));
            _grilla.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) Seleccionar(); };

            _lblResumen = new Label
            {
                Location = new Point(18, 366),
                Size = new Size(380, 22),
                ForeColor = Tema.TextoSuave
            };

            var btnSeleccionar = new Button { Text = "Seleccionar", Location = new Point(404, 362), Size = new Size(120, 34) };
            btnSeleccionar.Click += (s, e) => Seleccionar();
            var btnCancelar = new Button { Text = "Cancelar", Location = new Point(532, 362), Size = new Size(82, 34) };
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; };

            _txtBusqueda.KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                Buscar();
            };

            Controls.AddRange(new Control[] { lblBuscar, _txtBusqueda, btnBuscar, _grilla, _lblResumen, btnSeleccionar, btnCancelar });

            ClientSize = new Size(632, 410);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Text = "Buscar paciente";
            AcceptButton = btnBuscar;
            CancelButton = btnCancelar;

            Tema.AplicarFormulario(this);
            Tema.CampoTexto(_txtBusqueda);
            Tema.Grilla(_grilla);
            Tema.BotonSecundario(btnBuscar);
            Tema.BotonPrimario(btnSeleccionar);
            Tema.BotonSecundario(btnCancelar);

            Shown += (s, e) => Buscar();
        }

        private static DataGridViewTextBoxColumn NuevaColumna(string propiedad, string titulo, int peso)
        {
            return new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo,
                FillWeight = peso,
                ReadOnly = true
            };
        }

        private void Buscar()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                List<Paciente> pacientes = _servicio.BuscarPacientes(_txtBusqueda.Text);
                _grilla.DataSource = pacientes;
                _lblResumen.Text = pacientes.Count == 0
                    ? "Ningún paciente coincide con la búsqueda."
                    : pacientes.Count + " paciente(s) encontrado(s).";
            }
            catch (Exception ex)
            {
                Registro.Error("FrmBuscarPaciente.Buscar", ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void Seleccionar()
        {
            if (_grilla.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un paciente de la lista.", "Sin selección",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PacienteSeleccionado = _grilla.CurrentRow.DataBoundItem as Paciente;
            DialogResult = DialogResult.OK;
        }
    }
}
