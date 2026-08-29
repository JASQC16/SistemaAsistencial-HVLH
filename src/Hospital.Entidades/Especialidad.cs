namespace Hospital.Entidades
{
    /// <summary>Servicio o especialidad asistencial a la que pertenece un profesional.</summary>
    public class Especialidad
    {
        public int IdEspecialidad { get; set; }
        public string Nombre { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
