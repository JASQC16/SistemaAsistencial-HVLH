using System;
using Hospital.AccesoDatos;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.Negocio
{
    public class UsuarioServicio
    {
        private readonly UsuarioRepositorio _repositorio = new UsuarioRepositorio();

        /// <summary>
        /// Valida credenciales. El mensaje de error es el mismo para usuario inexistente
        /// y clave incorrecta, para no revelar qué usuarios existen en el sistema.
        /// </summary>
        public Usuario Autenticar(string nombreUsuario, string clave)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new NegocioException("Ingrese su usuario.");
            if (string.IsNullOrWhiteSpace(clave))
                throw new NegocioException("Ingrese su contraseña.");

            Usuario usuario = _repositorio.ObtenerPorNombre(nombreUsuario.Trim());

            if (usuario == null || !Seguridad.VerificarClave(clave, usuario.ClaveSalt, usuario.ClaveHash))
                throw new NegocioException("Usuario o contraseña incorrectos.");

            if (!usuario.Activo)
                throw new NegocioException("El usuario se encuentra inactivo. Comuníquese con el administrador.");

            _repositorio.RegistrarAcceso(usuario.IdUsuario);
            Registro.Informacion("Ingreso al sistema: " + usuario.NombreUsuario);
            return usuario;
        }
    }
}
