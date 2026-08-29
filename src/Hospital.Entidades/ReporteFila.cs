using System;

namespace Hospital.Entidades
{
    /// <summary>
    /// Fila del reporte de pacientes y atenciones. Representa un encuentro
    /// asistencial: una cita del periodo (con o sin atención) o una atención por
    /// demanda espontánea, es decir sin cita previa.
    /// </summary>
    public class ReporteFila
    {
        /// <summary>CITA o ESPONTANEA.</summary>
        public string Origen { get; set; }

        public string NumeroCita { get; set; }
        public DateTime? FechaCita { get; set; }
        public string NumeroAtencion { get; set; }
        public DateTime? FechaAtencion { get; set; }
        public DateTime FechaReferencia { get; set; }

        public string EstadoCita { get; set; }
        public string EstadoDescripcion { get; set; }
        public string MotivoEstado { get; set; }
        public string Motivo { get; set; }

        public string TipoDocumento { get; set; }
        public string DocumentoPaciente { get; set; }
        public string HistoriaClinica { get; set; }
        public string Paciente { get; set; }
        public string Sexo { get; set; }
        public int EdadPaciente { get; set; }

        public string Medico { get; set; }
        public string Especialidad { get; set; }
        public string Diagnosticos { get; set; }
    }
}
