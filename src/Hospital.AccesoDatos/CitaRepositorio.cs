using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.AccesoDatos
{
    /// <summary>
    /// Acceso a datos de la agenda de citas.
    ///
    /// El desenlace de una cita (atendido, no atendido, no acudió, cancelado) es un
    /// dato que alguien registra explícitamente. El sistema nunca lo deduce: que no
    /// exista una atención no convierte una cita en inasistencia.
    /// </summary>
    public class CitaRepositorio
    {
        public List<Cita> Listar(FiltroCita filtro)
        {
            var lista = new List<Cita>();
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Cita_Listar", conexion))
                {
                    SqlAyudante.Agregar(comando, "@FechaDesde", SqlDbType.Date, filtro.FechaDesde);
                    SqlAyudante.Agregar(comando, "@FechaHasta", SqlDbType.Date, filtro.FechaHasta);
                    SqlAyudante.Agregar(comando, "@Busqueda", SqlDbType.NVarChar, filtro.Busqueda);
                    SqlAyudante.Agregar(comando, "@IdMedico", SqlDbType.Int, filtro.IdMedico);
                    SqlAyudante.Agregar(comando, "@IdEspecialidad", SqlDbType.Int, filtro.IdEspecialidad);
                    SqlAyudante.Agregar(comando, "@Estado", SqlDbType.VarChar, filtro.Estado);

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
                Registro.Error("CitaRepositorio.Listar", ex);
                throw ErroresSql.Traducir(ex, "No fue posible consultar las citas.");
            }
        }

        public Cita ObtenerPorId(int idCita)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Cita_ObtenerPorId", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdCita", SqlDbType.Int, idCita);
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        return lector.Read() ? Mapear(lector) : null;
                    }
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("CitaRepositorio.ObtenerPorId", ex);
                throw ErroresSql.Traducir(ex, "No fue posible cargar la cita seleccionada.");
            }
        }

        /// <summary>
        /// Citas del paciente que todavía pueden convertirse en atención. Alimenta el
        /// combo del formulario de atenciones para enlazar el acto clínico con la cita
        /// que lo originó.
        /// </summary>
        public List<Cita> ListarPendientesPorPaciente(int idPaciente, int? idCitaActual)
        {
            var lista = new List<Cita>();
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Cita_ListarPendientesPorPaciente", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdPaciente", SqlDbType.Int, idPaciente);
                    SqlAyudante.Agregar(comando, "@IdCitaActual", SqlDbType.Int, idCitaActual);

                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Cita
                            {
                                IdCita       = SqlAyudante.LeerEntero(lector, "IdCita"),
                                NumeroCita   = SqlAyudante.LeerTexto(lector, "NumeroCita"),
                                FechaCita    = SqlAyudante.LeerFecha(lector, "FechaCita"),
                                MotivoCita   = SqlAyudante.LeerTexto(lector, "MotivoCita"),
                                Estado       = SqlAyudante.LeerTexto(lector, "Estado"),
                                Medico       = SqlAyudante.LeerTexto(lector, "Medico"),
                                Especialidad = SqlAyudante.LeerTexto(lector, "Especialidad")
                            });
                        }
                    }
                }
                return lista;
            }
            catch (SqlException ex)
            {
                Registro.Error("CitaRepositorio.ListarPendientesPorPaciente", ex);
                throw ErroresSql.Traducir(ex, "No fue posible obtener las citas del paciente.");
            }
        }

        public int Insertar(Cita cita)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Cita_Insertar", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdPaciente", SqlDbType.Int, cita.IdPaciente);
                    SqlAyudante.Agregar(comando, "@IdMedico", SqlDbType.Int, cita.IdMedico);
                    SqlAyudante.Agregar(comando, "@FechaCita", SqlDbType.DateTime2, cita.FechaCita);
                    SqlAyudante.Agregar(comando, "@MotivoCita", SqlDbType.NVarChar, cita.MotivoCita);
                    SqlAyudante.Agregar(comando, "@Observaciones", SqlDbType.NVarChar, cita.Observaciones);
                    SqlAyudante.Agregar(comando, "@IdUsuarioRegistro", SqlDbType.Int, cita.IdUsuarioRegistro);

                    var pId = comando.Parameters.Add("@IdCita", SqlDbType.Int);
                    pId.Direction = ParameterDirection.Output;
                    var pNumero = comando.Parameters.Add("@NumeroCita", SqlDbType.VarChar, 15);
                    pNumero.Direction = ParameterDirection.Output;

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    cita.IdCita     = Convert.ToInt32(pId.Value);
                    cita.NumeroCita = Convert.ToString(pNumero.Value);
                    return cita.IdCita;
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("CitaRepositorio.Insertar", ex);
                throw ErroresSql.Traducir(ex, "No fue posible registrar la cita.");
            }
        }

        public void Actualizar(Cita cita)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Cita_Actualizar", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdCita", SqlDbType.Int, cita.IdCita);
                    SqlAyudante.Agregar(comando, "@IdPaciente", SqlDbType.Int, cita.IdPaciente);
                    SqlAyudante.Agregar(comando, "@IdMedico", SqlDbType.Int, cita.IdMedico);
                    SqlAyudante.Agregar(comando, "@FechaCita", SqlDbType.DateTime2, cita.FechaCita);
                    SqlAyudante.Agregar(comando, "@MotivoCita", SqlDbType.NVarChar, cita.MotivoCita);
                    SqlAyudante.Agregar(comando, "@Observaciones", SqlDbType.NVarChar, cita.Observaciones);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("CitaRepositorio.Actualizar", ex);
                throw ErroresSql.Traducir(ex, "No fue posible actualizar la cita.");
            }
        }

        /// <summary>
        /// Registra el desenlace de la cita. El estado ATENDIDO no se asigna por esta
        /// vía: lo fija el registro de la atención, de manera que sea imposible marcar
        /// como atendida una cita sin acto clínico detrás.
        /// </summary>
        public void CambiarEstado(int idCita, string estado, string motivo)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Cita_CambiarEstado", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdCita", SqlDbType.Int, idCita);
                    SqlAyudante.Agregar(comando, "@Estado", SqlDbType.VarChar, estado);
                    SqlAyudante.Agregar(comando, "@MotivoEstado", SqlDbType.NVarChar, motivo);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("CitaRepositorio.CambiarEstado", ex);
                throw ErroresSql.Traducir(ex, "No fue posible cambiar el estado de la cita.");
            }
        }

        private static Cita Mapear(IDataRecord registro)
        {
            return new Cita
            {
                IdCita            = SqlAyudante.LeerEntero(registro, "IdCita"),
                NumeroCita        = SqlAyudante.LeerTexto(registro, "NumeroCita"),
                FechaCita         = SqlAyudante.LeerFecha(registro, "FechaCita"),
                IdPaciente        = SqlAyudante.LeerEntero(registro, "IdPaciente"),
                TipoDocumento     = SqlAyudante.LeerTexto(registro, "TipoDocumento"),
                DocumentoPaciente = SqlAyudante.LeerTexto(registro, "DocumentoPaciente"),
                HistoriaClinica   = SqlAyudante.LeerTexto(registro, "HistoriaClinica"),
                Paciente          = SqlAyudante.LeerTexto(registro, "Paciente"),
                Sexo              = SqlAyudante.LeerTexto(registro, "Sexo"),
                EdadPaciente      = SqlAyudante.LeerEntero(registro, "EdadPaciente"),
                IdMedico          = SqlAyudante.LeerEntero(registro, "IdMedico"),
                Medico            = SqlAyudante.LeerTexto(registro, "Medico"),
                IdEspecialidad    = SqlAyudante.LeerEntero(registro, "IdEspecialidad"),
                Especialidad      = SqlAyudante.LeerTexto(registro, "Especialidad"),
                MotivoCita        = SqlAyudante.LeerTexto(registro, "MotivoCita"),
                Estado            = SqlAyudante.LeerTexto(registro, "Estado"),
                EstadoDescripcion = SqlAyudante.LeerTexto(registro, "EstadoDescripcion"),
                MotivoEstado      = SqlAyudante.LeerTexto(registro, "MotivoEstado"),
                Observaciones     = SqlAyudante.LeerTexto(registro, "Observaciones"),
                IdAtencion        = SqlAyudante.LeerEnteroNulable(registro, "IdAtencion"),
                NumeroAtencion    = SqlAyudante.LeerTexto(registro, "NumeroAtencion"),
                FechaAtencion     = SqlAyudante.LeerFechaNulable(registro, "FechaAtencion"),
                UsuarioRegistro   = SqlAyudante.LeerTexto(registro, "UsuarioRegistro"),
                FechaRegistro     = SqlAyudante.LeerFecha(registro, "FechaRegistro"),
                FechaModificacion = SqlAyudante.LeerFechaNulable(registro, "FechaModificacion")
            };
        }
    }
}
