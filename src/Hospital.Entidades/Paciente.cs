using System;

namespace Hospital.Entidades
{
    /// <summary>
    /// Paciente del hospital. Es la entidad raíz del modelo asistencial:
    /// PACIENTE -> CITAS -> ATENCIONES. Un paciente se registra una sola vez y se
    /// reutiliza en todas sus citas y atenciones; nunca se duplica por encuentro.
    /// </summary>
    public class Paciente
    {
        public Paciente()
        {
            TipoDocumento = "DNI";
            Sexo = "M";
            Activo = true;
            FechaNacimiento = DateTime.Today.AddYears(-30);
        }

        public int IdPaciente { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string HistoriaClinica { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }

        /// <summary>Contadores de solo lectura que devuelve el listado, para dar contexto en la grilla.</summary>
        public int TotalAtenciones { get; set; }
        public int TotalCitas { get; set; }

        public string NombreCompleto
        {
            get { return string.Format("{0} {1}, {2}", ApellidoPaterno, ApellidoMaterno, Nombres).Trim(); }
        }

        public string TipoDocumentoDescripcion
        {
            get
            {
                switch (TipoDocumento)
                {
                    case "DNI": return "DNI";
                    case "CE":  return "Carné de extranjería";
                    case "PAS": return "Pasaporte";
                    case "CNV": return "Certificado de nacido vivo";
                    case "OTR": return "Otro";
                    default:    return TipoDocumento;
                }
            }
        }

        public string SexoDescripcion
        {
            get { return Sexo == "F" ? "Femenino" : "Masculino"; }
        }

        public string EstadoDescripcion
        {
            get { return Activo ? "Activo" : "Inactivo"; }
        }

        /// <summary>
        /// Edad a la fecha actual. Se calcula aquí y no se guarda en la base porque
        /// una edad almacenada queda obsoleta al día siguiente del cumpleaños.
        /// </summary>
        public int Edad
        {
            get
            {
                int edad = DateTime.Today.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;
                return edad < 0 ? 0 : edad;
            }
        }

        public override string ToString()
        {
            return NombreCompleto;
        }
    }
}
