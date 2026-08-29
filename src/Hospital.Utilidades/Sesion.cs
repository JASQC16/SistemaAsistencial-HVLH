using System;

namespace Hospital.Utilidades
{
    /// <summary>Contexto del usuario autenticado durante la ejecución de la aplicación.</summary>
    public static class Sesion
    {
        public static int IdUsuario { get; private set; }
        public static string NombreUsuario { get; private set; }
        public static string NombreCompleto { get; private set; }
        public static string Rol { get; private set; }

        public static bool Activa { get { return IdUsuario > 0; } }

        public static void Iniciar(int idUsuario, string nombreUsuario, string nombreCompleto, string rol)
        {
            IdUsuario = idUsuario;
            NombreUsuario = nombreUsuario;
            NombreCompleto = nombreCompleto;
            Rol = rol;
        }

        public static void Cerrar()
        {
            IdUsuario = 0;
            NombreUsuario = null;
            NombreCompleto = null;
            Rol = null;
        }

        public static bool EsAdministrador
        {
            get { return string.Equals(Rol, "ADMINISTRADOR", StringComparison.OrdinalIgnoreCase); }
        }
    }
}
