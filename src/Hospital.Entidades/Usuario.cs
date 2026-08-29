using System;

namespace Hospital.Entidades
{
    /// <summary>Usuario del sistema. La clave nunca se almacena ni viaja en texto plano.</summary>
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string ClaveHash { get; set; }
        public string ClaveSalt { get; set; }
        public string NombreCompleto { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }
}
