namespace Hospital.Entidades
{
    /// <summary>
    /// Diagnóstico del catálogo CIE-10 oficial del MINSA, almacenado localmente en
    /// dbo.CatalogoCie10. La búsqueda se resuelve contra SQL Server, no contra
    /// Internet, de modo que responde en milisegundos y funciona sin conexión.
    /// </summary>
    public class DiagnosticoCie10
    {
        /// <summary>Código normalizado sin punto, tal como lo almacena el HIS-MINSA (F200).</summary>
        public string Codigo { get; set; }

        /// <summary>Código con punto para mostrar al usuario (F20.0).</summary>
        public string CodigoFormato { get; set; }

        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public string Grupo { get; set; }
        public string Capitulo { get; set; }
        public string CapituloNombre { get; set; }

        /// <summary>Restricción de sexo del diagnóstico según la tabla maestra ('M', 'F' o null).</summary>
        public string Sexo { get; set; }

        /// <summary>V = vigente, C = cesado por resolución ministerial.</summary>
        public string Estado { get; set; }

        public string VersionCatalogo { get; set; }

        public bool Vigente { get { return Estado != "C"; } }

        public string CodigoMostrar
        {
            get { return string.IsNullOrEmpty(CodigoFormato) ? Codigo : CodigoFormato; }
        }

        public override string ToString()
        {
            return CodigoMostrar + " - " + Descripcion;
        }
    }
}
