namespace Hospital.Entidades
{
    /// <summary>Diagnóstico CIE-10 asociado a una atención (línea de detalle).</summary>
    public class AtencionDetalle
    {
        public int IdAtencionDetalle { get; set; }
        public int IdAtencion { get; set; }
        public int Item { get; set; }
        public string CodigoCie10 { get; set; }
        public string DescripcionDiagnostico { get; set; }
        public string TipoDiagnostico { get; set; }
        public string Indicaciones { get; set; }

        /// <summary>
        /// Versión del catálogo CIE-10 vigente cuando se registró el diagnóstico.
        /// La línea guarda su propia copia del código y de la descripción, de modo
        /// que una actualización posterior del catálogo del MINSA no reescribe lo
        /// que ya está documentado en la historia clínica.
        /// </summary>
        public string VersionCatalogoCie10 { get; set; }

        public string TipoDiagnosticoDescripcion
        {
            get
            {
                switch (TipoDiagnostico)
                {
                    case "P": return "Presuntivo";
                    case "D": return "Definitivo";
                    case "R": return "Repetitivo";
                    default:  return TipoDiagnostico;
                }
            }
        }
    }
}
