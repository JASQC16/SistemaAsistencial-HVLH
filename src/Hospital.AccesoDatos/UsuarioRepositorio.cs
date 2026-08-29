using System;
using System.Data;
using System.Data.SqlClient;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.AccesoDatos
{
    public class UsuarioRepositorio
    {
        public Usuario ObtenerPorNombre(string nombreUsuario)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Usuario_ObtenerPorNombre", conexion))
                {
                    SqlAyudante.Agregar(comando, "@NombreUsuario", SqlDbType.VarChar, nombreUsuario);
                    conexion.Open();

                    using (var lector = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!lector.Read()) return null;

                        return new Usuario
                        {
                            IdUsuario      = SqlAyudante.LeerEntero(lector, "IdUsuario"),
                            NombreUsuario  = SqlAyudante.LeerTexto(lector, "NombreUsuario"),
                            ClaveHash      = SqlAyudante.LeerTexto(lector, "ClaveHash"),
                            ClaveSalt      = SqlAyudante.LeerTexto(lector, "ClaveSalt"),
                            NombreCompleto = SqlAyudante.LeerTexto(lector, "NombreCompleto"),
                            Rol            = SqlAyudante.LeerTexto(lector, "Rol"),
                            Activo         = SqlAyudante.LeerBooleano(lector, "Activo"),
                            UltimoAcceso   = SqlAyudante.LeerFechaNulable(lector, "UltimoAcceso")
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("UsuarioRepositorio.ObtenerPorNombre", ex);
                throw new DatosException("No fue posible validar las credenciales contra la base de datos.", ex);
            }
        }

        public void RegistrarAcceso(int idUsuario)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Usuario_RegistrarAcceso", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdUsuario", SqlDbType.Int, idUsuario);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                // El acceso ya fue validado: no se interrumpe el ingreso por un fallo de auditoría.
                Registro.Error("UsuarioRepositorio.RegistrarAcceso", ex);
            }
        }
    }
}
