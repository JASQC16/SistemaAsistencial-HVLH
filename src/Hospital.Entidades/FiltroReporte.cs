using System;

namespace Hospital.Entidades
{
    /// <summary>
    /// Criterios del reporte de pacientes y atenciones. El rango de fechas es
    /// obligatorio; el resto de campos son opcionales y se combinan entre sí.
    /// </summary>
    public class FiltroReporte
    {
        public FiltroReporte()
        {
            FechaHasta = DateTime.Today;
            FechaDesde = DateTime.Today.AddDays(-30);
        }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        /// <summary>Estado de la cita; null equivale a "todos".</summary>
        public string Estado { get; set; }
        public string Documento { get; set; }
        public int? IdMedico { get; set; }
        public int? IdEspecialidad { get; set; }
        public string CodigoCie10 { get; set; }

        // Textos que se envían al reporte para dejar constancia de lo que se filtró.
        public string EstadoDescripcion { get; set; }
        public string MedicoDescripcion { get; set; }
        public string EspecialidadDescripcion { get; set; }

        /// <summary>Resumen legible del filtro aplicado, que se imprime en la cabecera del RDLC.</summary>
        public string DescripcionFiltro()
        {
            var partes = new System.Collections.Generic.List<string>();
            partes.Add("Estado: " + (string.IsNullOrEmpty(EstadoDescripcion) ? "Todos" : EstadoDescripcion));
            if (!string.IsNullOrWhiteSpace(MedicoDescripcion))       partes.Add("Profesional: " + MedicoDescripcion);
            if (!string.IsNullOrWhiteSpace(EspecialidadDescripcion)) partes.Add("Servicio: " + EspecialidadDescripcion);
            if (!string.IsNullOrWhiteSpace(Documento))               partes.Add("Documento: " + Documento);
            if (!string.IsNullOrWhiteSpace(CodigoCie10))             partes.Add("CIE-10: " + CodigoCie10);
            return string.Join("   |   ", partes.ToArray());
        }
    }
}
