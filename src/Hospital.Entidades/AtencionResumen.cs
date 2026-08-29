using System;

namespace Hospital.Entidades
{
    /// <summary>Proyección de solo lectura que alimenta la grilla de consulta (vw_AtencionResumen).</summary>
    public class AtencionResumen
    {
        public int IdAtencion { get; set; }
        public string NumeroAtencion { get; set; }
        public DateTime FechaAtencion { get; set; }
        public string TipoDocumento { get; set; }
        public string DocumentoPaciente { get; set; }
        public string HistoriaClinica { get; set; }
        public string NumeroCita { get; set; }
        public string Paciente { get; set; }
        public int EdadPaciente { get; set; }
        public string Medico { get; set; }
        public string Especialidad { get; set; }
        public string MotivoConsulta { get; set; }
        public string Estado { get; set; }
        public string EstadoDescripcion { get; set; }
        public int TotalDiagnosticos { get; set; }
        public string UsuarioRegistro { get; set; }
    }
}
