using System;

namespace Hospital.Entidades
{
    /// <summary>Criterios de la consulta de citas.</summary>
    public class FiltroCita
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string Busqueda { get; set; }
        public int? IdMedico { get; set; }
        public int? IdEspecialidad { get; set; }
        public string Estado { get; set; }
    }
}
