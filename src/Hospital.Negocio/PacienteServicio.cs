using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Hospital.AccesoDatos;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.Negocio
{
    /// <summary>
    /// Reglas de negocio del maestro de pacientes.
    ///
    /// La regla central del módulo es que un paciente se registra una sola vez: antes
    /// de dar de alta a alguien se comprueba que su documento no esté ya en el
    /// sistema, para no terminar con el mismo paciente duplicado una vez por cada
    /// atención, que es exactamente lo que ocurre cuando el registro se improvisa
    /// desde el formulario de atención.
    /// </summary>
    public class PacienteServicio
    {
        private readonly PacienteRepositorio _repositorio = new PacienteRepositorio();

        /// <summary>Longitud exigida para cada tipo de documento de identidad.</summary>
        private static readonly Dictionary<string, int> LongitudDocumento = new Dictionary<string, int>
        {
            { "DNI", 8 },
            { "CE",  12 },
            { "PAS", 12 },
            { "CNV", 10 },
            { "OTR", 15 }
        };

        public List<Paciente> Listar(string busqueda, string tipoDocumento, bool? activo)
        {
            return _repositorio.Listar(
                string.IsNullOrWhiteSpace(busqueda) ? null : busqueda.Trim(),
                string.IsNullOrWhiteSpace(tipoDocumento) ? null : tipoDocumento,
                activo);
        }

        public List<Paciente> Buscar(string busqueda)
        {
            return _repositorio.Buscar(string.IsNullOrWhiteSpace(busqueda) ? null : busqueda.Trim());
        }

        public Paciente ObtenerPorId(int idPaciente)
        {
            if (idPaciente <= 0) throw new NegocioException("Debe seleccionar un paciente.");

            var paciente = _repositorio.ObtenerPorId(idPaciente);
            if (paciente == null) throw new NegocioException("El paciente ya no se encuentra registrado.");
            return paciente;
        }

        public int Registrar(Paciente paciente)
        {
            Validar(paciente, esNuevo: true);
            VerificarDuplicado(paciente, null);
            return _repositorio.Insertar(paciente);
        }

        public void Actualizar(Paciente paciente)
        {
            if (paciente.IdPaciente <= 0)
                throw new NegocioException("No se identificó el paciente que se intenta modificar.");

            Validar(paciente, esNuevo: false);
            VerificarDuplicado(paciente, paciente.IdPaciente);
            _repositorio.Actualizar(paciente);
        }

        public void CambiarEstado(int idPaciente, bool activo)
        {
            if (idPaciente <= 0) throw new NegocioException("Debe seleccionar un paciente.");
            _repositorio.CambiarEstado(idPaciente, activo);
        }

        /// <summary>
        /// Comprobación previa de duplicados. La garantía definitiva la da la
        /// restricción UNIQUE de la base; esto existe para poder avisar con el nombre
        /// del paciente ya registrado en lugar de mostrar un error de motor.
        /// </summary>
        private void VerificarDuplicado(Paciente paciente, int? idExcluir)
        {
            var existente = _repositorio.BuscarPorDocumento(
                paciente.TipoDocumento, paciente.NumeroDocumento, idExcluir);

            if (existente == null) return;

            string mensaje = string.Format(
                "Ya existe un paciente registrado con {0} {1}: {2} (historia clínica {3}).",
                paciente.TipoDocumentoDescripcion,
                paciente.NumeroDocumento,
                existente.NombreCompleto,
                existente.HistoriaClinica);

            if (!existente.Activo)
                mensaje += " Ese registro está inactivo; puede reactivarlo en lugar de crear uno nuevo.";

            throw new NegocioException(mensaje);
        }

        /// <summary>
        /// Acumula todos los errores y los presenta juntos. Corregir de a uno, con un
        /// mensaje por intento de guardado, es innecesariamente tedioso para quien
        /// registra decenas de pacientes al día.
        /// </summary>
        private static void Validar(Paciente paciente, bool esNuevo)
        {
            if (paciente == null) throw new NegocioException("No se recibió información del paciente.");

            var errores = new List<string>();

            // --- Documento de identidad ---
            if (string.IsNullOrWhiteSpace(paciente.TipoDocumento))
                errores.Add("Seleccione el tipo de documento.");
            else if (!LongitudDocumento.ContainsKey(paciente.TipoDocumento))
                errores.Add("El tipo de documento seleccionado no es válido.");

            if (string.IsNullOrWhiteSpace(paciente.NumeroDocumento))
            {
                errores.Add("El número de documento es obligatorio.");
            }
            else
            {
                string numero = paciente.NumeroDocumento.Trim();

                if (paciente.TipoDocumento == "DNI")
                {
                    if (!Regex.IsMatch(numero, @"^\d{8}$"))
                        errores.Add("El DNI debe tener exactamente 8 dígitos numéricos.");
                }
                else if (!Regex.IsMatch(numero, @"^[A-Za-z0-9\-]{6,15}$"))
                {
                    errores.Add("El número de documento debe tener entre 6 y 15 caracteres alfanuméricos.");
                }

                int maximo;
                if (paciente.TipoDocumento != null &&
                    LongitudDocumento.TryGetValue(paciente.TipoDocumento, out maximo) && numero.Length > maximo)
                {
                    errores.Add(string.Format("El documento de tipo {0} no puede exceder {1} caracteres.",
                                              paciente.TipoDocumento, maximo));
                }
            }

            // --- Identificación de la persona ---
            if (string.IsNullOrWhiteSpace(paciente.Nombres))
                errores.Add("Los nombres son obligatorios.");
            else if (paciente.Nombres.Trim().Length < 2)
                errores.Add("Los nombres deben tener al menos 2 caracteres.");

            if (string.IsNullOrWhiteSpace(paciente.ApellidoPaterno))
                errores.Add("El apellido paterno es obligatorio.");
            else if (paciente.ApellidoPaterno.Trim().Length < 2)
                errores.Add("El apellido paterno debe tener al menos 2 caracteres.");

            // --- Fecha de nacimiento ---
            if (paciente.FechaNacimiento == DateTime.MinValue)
            {
                errores.Add("La fecha de nacimiento es obligatoria.");
            }
            else if (paciente.FechaNacimiento.Date > DateTime.Today)
            {
                errores.Add("La fecha de nacimiento no puede ser posterior a hoy.");
            }
            else if (paciente.Edad > 120)
            {
                errores.Add("La fecha de nacimiento indica una edad mayor de 120 años. Verifique el dato.");
            }

            // --- Sexo ---
            if (paciente.Sexo != "M" && paciente.Sexo != "F")
                errores.Add("Seleccione el sexo del paciente.");

            // --- Datos de contacto (opcionales, pero con formato si se ingresan) ---
            if (!string.IsNullOrWhiteSpace(paciente.Telefono) &&
                !Regex.IsMatch(paciente.Telefono.Trim(), @"^[\d\s\-\+\(\)]{6,20}$"))
            {
                errores.Add("El teléfono solo admite dígitos, espacios y los signos + - ( ).");
            }

            if (!string.IsNullOrWhiteSpace(paciente.Correo) &&
                !Regex.IsMatch(paciente.Correo.Trim(), @"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$"))
            {
                errores.Add("El correo electrónico no tiene un formato válido.");
            }

            if (!string.IsNullOrWhiteSpace(paciente.Direccion) && paciente.Direccion.Trim().Length > 150)
                errores.Add("La dirección no puede exceder 150 caracteres.");

            if (errores.Count > 0) throw new NegocioException(Componer(errores));

            Normalizar(paciente);
        }

        /// <summary>
        /// Los datos se guardan normalizados: sin espacios sobrantes y con el documento
        /// en mayúsculas. Así "12345678 " y "12345678" no pueden convivir como si
        /// fueran dos pacientes distintos.
        /// </summary>
        private static void Normalizar(Paciente paciente)
        {
            paciente.NumeroDocumento = paciente.NumeroDocumento.Trim().ToUpperInvariant();
            paciente.Nombres         = Limpiar(paciente.Nombres);
            paciente.ApellidoPaterno = Limpiar(paciente.ApellidoPaterno);
            paciente.ApellidoMaterno = Limpiar(paciente.ApellidoMaterno);
            paciente.Telefono        = Limpiar(paciente.Telefono);
            paciente.Direccion       = Limpiar(paciente.Direccion);
            paciente.Correo          = string.IsNullOrWhiteSpace(paciente.Correo)
                                       ? null : paciente.Correo.Trim().ToLowerInvariant();
        }

        private static string Limpiar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;
            return Regex.Replace(texto.Trim(), @"\s{2,}", " ");
        }

        private static string Componer(List<string> errores)
        {
            var mensaje = new StringBuilder();
            mensaje.AppendLine("Revise los siguientes puntos antes de guardar:");
            mensaje.AppendLine();
            foreach (string error in errores) mensaje.AppendLine("  •  " + error);
            return mensaje.ToString();
        }
    }
}
