using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.AccesoDatos
{
    /// <summary>
    /// Acceso a datos del maestro de pacientes.
    ///
    /// El paciente es la entidad raíz del modelo: se registra una vez y se reutiliza
    /// en todas sus citas y atenciones. Por eso no hay eliminación física, solo
    /// activación y desactivación: borrar un paciente dejaría huérfana su historia.
    /// </summary>
    public class PacienteRepositorio
    {
        public List<Paciente> Listar(string busqueda, string tipoDocumento, bool? activo)
        {
            var lista = new List<Paciente>();
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Paciente_Listar", conexion))
                {
                    SqlAyudante.Agregar(comando, "@Busqueda", SqlDbType.NVarChar, busqueda);
                    SqlAyudante.Agregar(comando, "@TipoDocumento", SqlDbType.VarChar, tipoDocumento);
                    SqlAyudante.Agregar(comando, "@Activo", SqlDbType.Bit, activo);

                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            var paciente = Mapear(lector);
                            paciente.TotalAtenciones = SqlAyudante.LeerEntero(lector, "TotalAtenciones");
                            paciente.TotalCitas      = SqlAyudante.LeerEntero(lector, "TotalCitas");
                            lista.Add(paciente);
                        }
                    }
                }
                return lista;
            }
            catch (SqlException ex)
            {
                Registro.Error("PacienteRepositorio.Listar", ex);
                throw ErroresSql.Traducir(ex, "No fue posible consultar los pacientes.");
            }
        }

        /// <summary>Búsqueda rápida por prefijo, usada por el selector de pacientes.</summary>
        public List<Paciente> Buscar(string busqueda)
        {
            var lista = new List<Paciente>();
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Paciente_Buscar", conexion))
                {
                    SqlAyudante.Agregar(comando, "@Busqueda", SqlDbType.NVarChar, busqueda);
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read()) lista.Add(Mapear(lector));
                    }
                }
                return lista;
            }
            catch (SqlException ex)
            {
                Registro.Error("PacienteRepositorio.Buscar", ex);
                throw ErroresSql.Traducir(ex, "No fue posible realizar la búsqueda de pacientes.");
            }
        }

        public Paciente ObtenerPorId(int idPaciente)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Paciente_ObtenerPorId", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdPaciente", SqlDbType.Int, idPaciente);
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        return lector.Read() ? Mapear(lector) : null;
                    }
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("PacienteRepositorio.ObtenerPorId", ex);
                throw ErroresSql.Traducir(ex, "No fue posible cargar el paciente seleccionado.");
            }
        }

        /// <summary>
        /// Devuelve el paciente que ya tiene ese documento, o null si está libre.
        /// Permite avisar al usuario con nombre y número de historia antes de que
        /// intente guardar, en lugar de dejar que falle la restricción UNIQUE.
        /// </summary>
        public Paciente BuscarPorDocumento(string tipoDocumento, string numeroDocumento, int? idExcluir)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Paciente_ExisteDocumento", conexion))
                {
                    SqlAyudante.Agregar(comando, "@TipoDocumento", SqlDbType.VarChar, tipoDocumento);
                    SqlAyudante.Agregar(comando, "@NumeroDocumento", SqlDbType.VarChar, numeroDocumento);
                    SqlAyudante.Agregar(comando, "@IdPaciente", SqlDbType.Int, idExcluir);

                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        return lector.Read() ? Mapear(lector) : null;
                    }
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("PacienteRepositorio.BuscarPorDocumento", ex);
                throw ErroresSql.Traducir(ex, "No fue posible verificar el documento del paciente.");
            }
        }

        public int Insertar(Paciente paciente)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Paciente_Insertar", conexion))
                {
                    AgregarParametros(comando, paciente);

                    var pId = comando.Parameters.Add("@IdPaciente", SqlDbType.Int);
                    pId.Direction = ParameterDirection.Output;
                    var pHistoria = comando.Parameters.Add("@HistoriaClinica", SqlDbType.VarChar, 15);
                    pHistoria.Direction = ParameterDirection.Output;

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    paciente.IdPaciente      = Convert.ToInt32(pId.Value);
                    paciente.HistoriaClinica = Convert.ToString(pHistoria.Value);
                    return paciente.IdPaciente;
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("PacienteRepositorio.Insertar", ex);
                throw ErroresSql.Traducir(ex, "No fue posible registrar el paciente.");
            }
        }

        public void Actualizar(Paciente paciente)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Paciente_Actualizar", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdPaciente", SqlDbType.Int, paciente.IdPaciente);
                    AgregarParametros(comando, paciente);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("PacienteRepositorio.Actualizar", ex);
                throw ErroresSql.Traducir(ex, "No fue posible actualizar el paciente.");
            }
        }

        public void CambiarEstado(int idPaciente, bool activo)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Paciente_CambiarEstado", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdPaciente", SqlDbType.Int, idPaciente);
                    SqlAyudante.Agregar(comando, "@Activo", SqlDbType.Bit, activo);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("PacienteRepositorio.CambiarEstado", ex);
                throw ErroresSql.Traducir(ex, "No fue posible cambiar el estado del paciente.");
            }
        }

        private static void AgregarParametros(SqlCommand comando, Paciente paciente)
        {
            SqlAyudante.Agregar(comando, "@TipoDocumento", SqlDbType.VarChar, paciente.TipoDocumento);
            SqlAyudante.Agregar(comando, "@NumeroDocumento", SqlDbType.VarChar, paciente.NumeroDocumento);
            SqlAyudante.Agregar(comando, "@Nombres", SqlDbType.NVarChar, paciente.Nombres);
            SqlAyudante.Agregar(comando, "@ApellidoPaterno", SqlDbType.NVarChar, paciente.ApellidoPaterno);
            SqlAyudante.Agregar(comando, "@ApellidoMaterno", SqlDbType.NVarChar, paciente.ApellidoMaterno);
            SqlAyudante.Agregar(comando, "@FechaNacimiento", SqlDbType.Date, paciente.FechaNacimiento.Date);
            SqlAyudante.Agregar(comando, "@Sexo", SqlDbType.Char, paciente.Sexo);
            SqlAyudante.Agregar(comando, "@Telefono", SqlDbType.VarChar, paciente.Telefono);
            SqlAyudante.Agregar(comando, "@Direccion", SqlDbType.NVarChar, paciente.Direccion);
            SqlAyudante.Agregar(comando, "@Correo", SqlDbType.NVarChar, paciente.Correo);
        }

        private static Paciente Mapear(IDataRecord registro)
        {
            return new Paciente
            {
                IdPaciente        = SqlAyudante.LeerEntero(registro, "IdPaciente"),
                TipoDocumento     = SqlAyudante.LeerTexto(registro, "TipoDocumento"),
                NumeroDocumento   = SqlAyudante.LeerTexto(registro, "NumeroDocumento"),
                HistoriaClinica   = SqlAyudante.LeerTexto(registro, "HistoriaClinica"),
                Nombres           = SqlAyudante.LeerTexto(registro, "Nombres"),
                ApellidoPaterno   = SqlAyudante.LeerTexto(registro, "ApellidoPaterno"),
                ApellidoMaterno   = SqlAyudante.LeerTexto(registro, "ApellidoMaterno"),
                FechaNacimiento   = SqlAyudante.LeerFecha(registro, "FechaNacimiento"),
                Sexo              = SqlAyudante.LeerTexto(registro, "Sexo"),
                Telefono          = SqlAyudante.LeerTexto(registro, "Telefono"),
                Direccion         = SqlAyudante.LeerTexto(registro, "Direccion"),
                Correo            = SqlAyudante.LeerTexto(registro, "Correo"),
                Activo            = SqlAyudante.LeerBooleano(registro, "Activo"),
                FechaRegistro     = SqlAyudante.LeerFecha(registro, "FechaRegistro"),
                FechaModificacion = SqlAyudante.LeerFechaNulable(registro, "FechaModificacion")
            };
        }
    }
}
