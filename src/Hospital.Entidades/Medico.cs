namespace Hospital.Entidades
{
    public class Medico
    {
        public int IdMedico { get; set; }
        public string NumeroColegiatura { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public int IdEspecialidad { get; set; }
        public string Especialidad { get; set; }

        public string NombreCompleto
        {
            get { return string.Format("{0}, {1} ({2})", Apellidos, Nombres, Especialidad); }
        }
    }
}
