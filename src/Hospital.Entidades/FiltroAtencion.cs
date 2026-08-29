using System;

namespace Hospital.Entidades
{
    /// <summary>Agrupa los criterios de búsqueda para no propagar listas largas de parámetros.</summary>
    public class FiltroAtencion
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string Busqueda { get; set; }
        public int? IdMedico { get; set; }
        public string Estado { get; set; }
    }
}
