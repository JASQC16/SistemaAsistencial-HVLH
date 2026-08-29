using System;
using System.Collections.Generic;
using System.Text;
using Hospital.AccesoDatos;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.Negocio
{
    /// <summary>
    /// Reglas de negocio de la agenda de citas.
    ///
    /// El módulo existe para poder responder algo que las atenciones por sí solas no
    /// permiten: quién estaba citado, quién acudió y quién no. De ahí la regla que
    /// gobierna todo lo demás: el estado de una cita se registra, nunca se deduce.
    /// Que no exista una atención asociada no convierte a la cita en inasistencia;
    /// alguien tiene que marcarla como tal e indicar el motivo.
    /// </summary>
    public class CitaServicio
    {
        private readonly CitaRepositorio _repositorio = new CitaRepositorio();

        /// <summary>Horario de atención ambulatoria del hospital.</summary>
        private const int HoraApertura = 7;
        private const int HoraCierre   = 20;

        public List<Cita> Listar(FiltroCita filtro)
        {
            if (filtro == null) filtro = new FiltroCita();

            if (filtro.FechaDesde.HasValue && filtro.FechaHasta.HasValue &&
                filtro.FechaDesde.Value.Date > filtro.FechaHasta.Value.Date)
            {
                throw new NegocioException("La fecha inicial no puede ser posterior a la fecha final.");
            }

            return _repositorio.Listar(filtro);
        }

        public Cita ObtenerPorId(int idCita)
        {
            if (idCita <= 0) throw new NegocioException("Debe seleccionar una cita.");

            var cita = _repositorio.ObtenerPorId(idCita);
            if (cita == null) throw new NegocioException("La cita ya no se encuentra registrada.");
            return cita;
        }

        public List<Cita> ListarPendientesPorPaciente(int idPaciente, int? idCitaActual)
        {
            if (idPaciente <= 0) return new List<Cita>();
            return _repositorio.ListarPendientesPorPaciente(idPaciente, idCitaActual);
        }

        public int Registrar(Cita cita)
        {
            Validar(cita, esNueva: true);
            return _repositorio.Insertar(cita);
        }

        public void Actualizar(Cita cita)
        {
            if (cita.IdCita <= 0)
                throw new NegocioException("No se identificó la cita que se intenta modificar.");

            Validar(cita, esNueva: false);
            _repositorio.Actualizar(cita);
        }

        /// <summary>
        /// Registra el desenlace de la cita. El estado ATENDIDO se excluye
        /// deliberadamente: lo asigna el registro de la atención, de manera que no
        /// exista forma de declarar atendido a alguien sin un acto clínico detrás.
        /// </summary>
        public void CambiarEstado(Cita cita, string nuevoEstado, string motivo)
        {
            if (cita == null || cita.IdCita <= 0)
                throw new NegocioException("Debe seleccionar una cita.");

            if (cita.Estado == EstadoCita.Atendido)
                throw new NegocioException(
                    "La cita ya fue atendida. Si necesita revertirla, anule primero la atención asociada.");

            if (nuevoEstado == EstadoCita.Atendido)
                throw new NegocioException(
                    "El estado Atendido no se asigna manualmente: se establece al registrar la atención del paciente.");

            if (nuevoEstado != EstadoCita.Citado &&
                nuevoEstado != EstadoCita.NoAtendido &&
                nuevoEstado != EstadoCita.NoAcudio &&
                nuevoEstado != EstadoCita.Cancelado)
            {
                throw new NegocioException("El estado seleccionado no es válido.");
            }

            if (nuevoEstado != EstadoCita.Citado &&
                (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length < 5))
            {
                throw new NegocioException("Indique el motivo con al menos 5 caracteres.");
            }

            // Marcar una inasistencia de una cita que aún no ha llegado carece de
            // sentido y contamina los indicadores del servicio.
            if (nuevoEstado == EstadoCita.NoAcudio && cita.FechaCita > DateTime.Now)
                throw new NegocioException(
                    "No puede registrarse una inasistencia de una cita cuya fecha y hora aún no han llegado.");

            _repositorio.CambiarEstado(cita.IdCita, nuevoEstado,
                                       nuevoEstado == EstadoCita.Citado ? null : motivo.Trim());
        }

        private static void Validar(Cita cita, bool esNueva)
        {
            if (cita == null) throw new NegocioException("No se recibió información de la cita.");

            var errores = new List<string>();

            if (cita.IdPaciente <= 0) errores.Add("Seleccione el paciente.");
            if (cita.IdMedico <= 0)   errores.Add("Seleccione el profesional que atenderá la cita.");

            if (cita.FechaCita == DateTime.MinValue)
            {
                errores.Add("Indique la fecha y hora de la cita.");
            }
            else
            {
                // Al programar una cita nueva no tiene sentido hacerlo hacia atrás;
                // al editar una existente sí se admite corregir datos de una ya pasada.
                if (esNueva && cita.FechaCita < DateTime.Now.AddMinutes(-5))
                    errores.Add("No puede programarse una cita en una fecha y hora ya transcurridas.");

                if (cita.FechaCita > DateTime.Now.AddYears(1))
                    errores.Add("La cita no puede programarse con más de un año de anticipación.");

                if (cita.FechaCita.Hour < HoraApertura || cita.FechaCita.Hour >= HoraCierre)
                    errores.Add(string.Format(
                        "El horario de atención ambulatoria es de {0:00}:00 a {1:00}:00.", HoraApertura, HoraCierre));

                if (cita.FechaCita.DayOfWeek == DayOfWeek.Sunday)
                    errores.Add("No se programan citas de consulta externa los días domingo.");
            }

            if (!string.IsNullOrWhiteSpace(cita.MotivoCita) && cita.MotivoCita.Trim().Length > 300)
                errores.Add("El motivo de la cita no puede exceder 300 caracteres.");

            if (!string.IsNullOrWhiteSpace(cita.Observaciones) && cita.Observaciones.Trim().Length > 500)
                errores.Add("Las observaciones no pueden exceder 500 caracteres.");

            if (errores.Count > 0)
            {
                var mensaje = new StringBuilder();
                mensaje.AppendLine("Revise los siguientes puntos antes de guardar:");
                mensaje.AppendLine();
                foreach (string error in errores) mensaje.AppendLine("  •  " + error);
                throw new NegocioException(mensaje.ToString());
            }

            cita.MotivoCita    = Limpiar(cita.MotivoCita);
            cita.Observaciones = Limpiar(cita.Observaciones);

            // Los segundos se descartan: una agenda se programa en horas y minutos, y
            // conservarlos rompería la comparación de horarios duplicados.
            cita.FechaCita = new DateTime(cita.FechaCita.Year, cita.FechaCita.Month, cita.FechaCita.Day,
                                          cita.FechaCita.Hour, cita.FechaCita.Minute, 0);
        }

        private static string Limpiar(string texto)
        {
            return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
        }
    }
}
