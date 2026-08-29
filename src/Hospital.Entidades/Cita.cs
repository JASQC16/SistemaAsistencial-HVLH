using System;

namespace Hospital.Entidades
{
    /// <summary>
    /// Cita programada. Es el eslabón intermedio entre el paciente y la atención:
    /// permite saber quién estaba citado, quién acudió y quién no, algo imposible
    /// de responder mirando solo las atenciones registradas.
    /// </summary>
    public class Cita
    {
        public Cita()
        {
            FechaCita = DateTime.Today.AddDays(1).AddHours(9);
            Estado = EstadoCita.Citado;
        }

        public int IdCita { get; set; }
        public string NumeroCita { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public DateTime FechaCita { get; set; }
        public string MotivoCita { get; set; }
        public string Estado { get; set; }
        public string MotivoEstado { get; set; }
        public string Observaciones { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Campos de presentación que provienen de vw_CitaResumen.
        public string TipoDocumento { get; set; }
        public string DocumentoPaciente { get; set; }
        public string HistoriaClinica { get; set; }
        public string Paciente { get; set; }
        public string Sexo { get; set; }
        public int EdadPaciente { get; set; }
        public string Medico { get; set; }
        public int IdEspecialidad { get; set; }
        public string Especialidad { get; set; }
        public string EstadoDescripcion { get; set; }
        public string UsuarioRegistro { get; set; }

        // Atención derivada de la cita, si llegó a producirse.
        public int? IdAtencion { get; set; }
        public string NumeroAtencion { get; set; }
        public DateTime? FechaAtencion { get; set; }

        public bool TieneAtencion { get { return IdAtencion.HasValue; } }

        /// <summary>Solo una cita en estado CITADO admite reprogramación o cambio de desenlace.</summary>
        public bool EsModificable { get { return Estado == EstadoCita.Citado; } }

        public override string ToString()
        {
            return string.Format("{0} - {1:dd/MM/yyyy HH:mm}", NumeroCita, FechaCita);
        }
    }

    /// <summary>
    /// Estados posibles de una cita. Se declaran como constantes en lugar de un enum
    /// porque viajan como texto hasta la restricción CHECK de la base de datos: así
    /// el valor de C# y el aceptado por SQL Server son literalmente el mismo.
    /// </summary>
    public static class EstadoCita
    {
        public const string Citado      = "CITADO";
        public const string Atendido    = "ATENDIDO";
        public const string NoAtendido  = "NO_ATENDIDO";
        public const string NoAcudio    = "NO_ACUDIO";
        public const string Cancelado   = "CANCELADO";

        public static string Descripcion(string estado)
        {
            switch (estado)
            {
                case Citado:     return "Citado";
                case Atendido:   return "Atendido";
                case NoAtendido: return "No atendido";
                case NoAcudio:   return "No acudió";
                case Cancelado:  return "Cancelado";
                default:         return estado;
            }
        }
    }
}
