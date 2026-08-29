using System;
using System.Collections.Generic;

namespace Hospital.Entidades
{
    /// <summary>
    /// Cabecera del proceso asistencial. Contiene su propia colección de detalles,
    /// de modo que la operación de guardado viaja como una sola unidad de trabajo.
    /// </summary>
    public class Atencion
    {
        public Atencion()
        {
            Detalles = new List<AtencionDetalle>();
            FechaAtencion = DateTime.Now;
            Estado = "R";
        }

        public int IdAtencion { get; set; }
        public string NumeroAtencion { get; set; }
        public int IdPaciente { get; set; }
        public string TipoDocumento { get; set; }
        public string DocumentoPaciente { get; set; }
        public string HistoriaClinica { get; set; }
        public string NombrePaciente { get; set; }
        public int IdMedico { get; set; }

        /// <summary>
        /// Cita que originó la atención. Es nullable a propósito: una atención puede
        /// nacer de una cita programada o de demanda espontánea, y el modelo debe
        /// admitir ambos casos sin obligar a inventar una cita ficticia.
        /// </summary>
        public int? IdCita { get; set; }
        public string NumeroCita { get; set; }
        public DateTime? FechaCita { get; set; }

        public DateTime FechaAtencion { get; set; }
        public string MotivoConsulta { get; set; }
        public decimal? Temperatura { get; set; }
        public string PresionArterial { get; set; }
        public int? FrecuenciaCardiaca { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Talla { get; set; }
        public string Observaciones { get; set; }
        public string Estado { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public List<AtencionDetalle> Detalles { get; set; }
    }
}
