using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.AccesoDatos
{
    /// <summary>
    /// Acceso a datos del proceso asistencial. Las operaciones que tocan cabecera y
    /// detalle se ejecutan dentro de una única SqlTransaction: o se guarda la atención
    /// completa con todos sus diagnósticos, o no se guarda nada.
    /// </summary>
    public class AtencionRepositorio
    {
        #region Consultas

        public List<AtencionResumen> Listar(FiltroAtencion filtro)
        {
            var lista = new List<AtencionResumen>();
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Atencion_Listar", conexion))
                {
                    SqlAyudante.Agregar(comando, "@FechaDesde", SqlDbType.Date, filtro.FechaDesde);
                    SqlAyudante.Agregar(comando, "@FechaHasta", SqlDbType.Date, filtro.FechaHasta);
                    SqlAyudante.Agregar(comando, "@Busqueda", SqlDbType.NVarChar, filtro.Busqueda);
                    SqlAyudante.Agregar(comando, "@IdMedico", SqlDbType.Int, filtro.IdMedico);
                    SqlAyudante.Agregar(comando, "@Estado", SqlDbType.Char, filtro.Estado);

                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new AtencionResumen
                            {
                                IdAtencion        = SqlAyudante.LeerEntero(lector, "IdAtencion"),
                                NumeroAtencion    = SqlAyudante.LeerTexto(lector, "NumeroAtencion"),
                                FechaAtencion     = SqlAyudante.LeerFecha(lector, "FechaAtencion"),
                                TipoDocumento     = SqlAyudante.LeerTexto(lector, "TipoDocumento"),
                                DocumentoPaciente = SqlAyudante.LeerTexto(lector, "DocumentoPaciente"),
                                HistoriaClinica   = SqlAyudante.LeerTexto(lector, "HistoriaClinica"),
                                NumeroCita        = SqlAyudante.LeerTexto(lector, "NumeroCita"),
                                Paciente          = SqlAyudante.LeerTexto(lector, "Paciente"),
                                EdadPaciente      = SqlAyudante.LeerEntero(lector, "EdadPaciente"),
                                Medico            = SqlAyudante.LeerTexto(lector, "Medico"),
                                Especialidad      = SqlAyudante.LeerTexto(lector, "Especialidad"),
                                MotivoConsulta    = SqlAyudante.LeerTexto(lector, "MotivoConsulta"),
                                Estado            = SqlAyudante.LeerTexto(lector, "Estado"),
                                EstadoDescripcion = SqlAyudante.LeerTexto(lector, "EstadoDescripcion"),
                                TotalDiagnosticos = SqlAyudante.LeerEntero(lector, "TotalDiagnosticos"),
                                UsuarioRegistro   = SqlAyudante.LeerTexto(lector, "UsuarioRegistro")
                            });
                        }
                    }
                }
                return lista;
            }
            catch (SqlException ex)
            {
                Registro.Error("AtencionRepositorio.Listar", ex);
                throw new DatosException("No fue posible consultar las atenciones.", ex);
            }
        }

        /// <summary>Devuelve la cabecera con su detalle (el SP retorna dos resultsets).</summary>
        public Atencion ObtenerPorId(int idAtencion)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Atencion_ObtenerPorId", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdAtencion", SqlDbType.Int, idAtencion);
                    conexion.Open();

                    using (var lector = comando.ExecuteReader())
                    {
                        if (!lector.Read()) return null;

                        var atencion = new Atencion
                        {
                            IdAtencion         = SqlAyudante.LeerEntero(lector, "IdAtencion"),
                            NumeroAtencion     = SqlAyudante.LeerTexto(lector, "NumeroAtencion"),
                            IdPaciente         = SqlAyudante.LeerEntero(lector, "IdPaciente"),
                            IdMedico           = SqlAyudante.LeerEntero(lector, "IdMedico"),
                            FechaAtencion      = SqlAyudante.LeerFecha(lector, "FechaAtencion"),
                            MotivoConsulta     = SqlAyudante.LeerTexto(lector, "MotivoConsulta"),
                            Temperatura        = SqlAyudante.LeerDecimalNulable(lector, "Temperatura"),
                            PresionArterial    = SqlAyudante.LeerTexto(lector, "PresionArterial"),
                            FrecuenciaCardiaca = SqlAyudante.LeerEnteroNulable(lector, "FrecuenciaCardiaca"),
                            Peso               = SqlAyudante.LeerDecimalNulable(lector, "Peso"),
                            Talla              = SqlAyudante.LeerDecimalNulable(lector, "Talla"),
                            Observaciones      = SqlAyudante.LeerTexto(lector, "Observaciones"),
                            Estado             = SqlAyudante.LeerTexto(lector, "Estado"),
                            IdUsuarioRegistro  = SqlAyudante.LeerEntero(lector, "IdUsuarioRegistro"),
                            FechaRegistro      = SqlAyudante.LeerFecha(lector, "FechaRegistro"),
                            FechaModificacion  = SqlAyudante.LeerFechaNulable(lector, "FechaModificacion"),
                            IdCita             = SqlAyudante.LeerEnteroNulable(lector, "IdCita"),
                            NumeroCita         = SqlAyudante.LeerTexto(lector, "NumeroCita"),
                            FechaCita          = SqlAyudante.LeerFechaNulable(lector, "FechaCita"),
                            TipoDocumento      = SqlAyudante.LeerTexto(lector, "TipoDocumento"),
                            DocumentoPaciente  = SqlAyudante.LeerTexto(lector, "DocumentoPaciente"),
                            HistoriaClinica    = SqlAyudante.LeerTexto(lector, "HistoriaClinica"),
                            NombrePaciente     = SqlAyudante.LeerTexto(lector, "NombrePaciente")
                        };

                        if (lector.NextResult())
                        {
                            while (lector.Read())
                            {
                                atencion.Detalles.Add(new AtencionDetalle
                                {
                                    IdAtencionDetalle      = SqlAyudante.LeerEntero(lector, "IdAtencionDetalle"),
                                    IdAtencion             = SqlAyudante.LeerEntero(lector, "IdAtencion"),
                                    Item                   = SqlAyudante.LeerEntero(lector, "Item"),
                                    CodigoCie10            = SqlAyudante.LeerTexto(lector, "CodigoCie10"),
                                    DescripcionDiagnostico = SqlAyudante.LeerTexto(lector, "DescripcionDiagnostico"),
                                    TipoDiagnostico        = SqlAyudante.LeerTexto(lector, "TipoDiagnostico"),
                                    Indicaciones           = SqlAyudante.LeerTexto(lector, "Indicaciones"),
                                    VersionCatalogoCie10   = SqlAyudante.LeerTexto(lector, "VersionCatalogoCie10")
                                });
                            }
                        }

                        return atencion;
                    }
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("AtencionRepositorio.ObtenerPorId", ex);
                throw new DatosException("No fue posible cargar la atención seleccionada.", ex);
            }
        }

        #endregion

        #region Escritura transaccional

        /// <summary>
        /// Inserta cabecera y detalle en una sola unidad de trabajo.
        /// El nivel de aislamiento ReadCommitted es suficiente: no hay relecturas
        /// dentro de la transacción, y evita el bloqueo excesivo de Serializable.
        /// </summary>
        public int Insertar(Atencion atencion)
        {
            using (var conexion = ConexionBD.Crear())
            {
                conexion.Open();
                using (var transaccion = conexion.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        int idAtencion;

                        using (var comando = SqlAyudante.CrearComando("dbo.usp_Atencion_Insertar", conexion, transaccion))
                        {
                            AgregarParametrosCabecera(comando, atencion);
                            SqlAyudante.Agregar(comando, "@IdUsuarioRegistro", SqlDbType.Int, atencion.IdUsuarioRegistro);

                            var pId = comando.Parameters.Add("@IdAtencion", SqlDbType.Int);
                            pId.Direction = ParameterDirection.Output;
                            var pNumero = comando.Parameters.Add("@NumeroAtencion", SqlDbType.VarChar, 15);
                            pNumero.Direction = ParameterDirection.Output;

                            comando.ExecuteNonQuery();

                            idAtencion = Convert.ToInt32(pId.Value);
                            atencion.IdAtencion = idAtencion;
                            atencion.NumeroAtencion = Convert.ToString(pNumero.Value);
                        }

                        InsertarDetalles(conexion, transaccion, idAtencion, atencion.Detalles);
                        SincronizarCita(conexion, transaccion, atencion);

                        transaccion.Commit();
                        return idAtencion;
                    }
                    catch (SqlException ex)
                    {
                        ErroresSql.Revertir(transaccion, "AtencionRepositorio.Insertar");
                        Registro.Error("AtencionRepositorio.Insertar", ex);
                        throw ErroresSql.Traducir(ex, "No fue posible registrar la atención.");
                    }
                    catch (Exception ex)
                    {
                        ErroresSql.Revertir(transaccion, "AtencionRepositorio.Insertar");
                        Registro.Error("AtencionRepositorio.Insertar", ex);
                        throw new DatosException("No fue posible registrar la atención.", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Actualiza la cabecera y reemplaza el detalle completo. Borrar e insertar
        /// dentro de la misma transacción es más simple y seguro que sincronizar
        /// línea por línea, y el volumen por atención es de pocos registros.
        /// </summary>
        public void Actualizar(Atencion atencion)
        {
            using (var conexion = ConexionBD.Crear())
            {
                conexion.Open();
                using (var transaccion = conexion.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        using (var comando = SqlAyudante.CrearComando("dbo.usp_Atencion_Actualizar", conexion, transaccion))
                        {
                            SqlAyudante.Agregar(comando, "@IdAtencion", SqlDbType.Int, atencion.IdAtencion);
                            AgregarParametrosCabecera(comando, atencion);
                            comando.ExecuteNonQuery();
                        }

                        using (var comando = SqlAyudante.CrearComando("dbo.usp_AtencionDetalle_EliminarPorAtencion", conexion, transaccion))
                        {
                            SqlAyudante.Agregar(comando, "@IdAtencion", SqlDbType.Int, atencion.IdAtencion);
                            comando.ExecuteNonQuery();
                        }

                        InsertarDetalles(conexion, transaccion, atencion.IdAtencion, atencion.Detalles);
                        SincronizarCita(conexion, transaccion, atencion);

                        transaccion.Commit();
                    }
                    catch (SqlException ex)
                    {
                        ErroresSql.Revertir(transaccion, "AtencionRepositorio.Actualizar");
                        Registro.Error("AtencionRepositorio.Actualizar", ex);
                        throw ErroresSql.Traducir(ex, "No fue posible actualizar la atención.");
                    }
                    catch (Exception ex)
                    {
                        ErroresSql.Revertir(transaccion, "AtencionRepositorio.Actualizar");
                        Registro.Error("AtencionRepositorio.Actualizar", ex);
                        throw new DatosException("No fue posible actualizar la atención.", ex);
                    }
                }
            }
        }

        public void Anular(int idAtencion, string motivo)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Atencion_Anular", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdAtencion", SqlDbType.Int, idAtencion);
                    SqlAyudante.Agregar(comando, "@Motivo", SqlDbType.NVarChar, motivo);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("AtencionRepositorio.Anular", ex);
                throw ErroresSql.Traducir(ex, "No fue posible anular la atención.");
            }
        }

        /// <summary>La transacción vive dentro del SP porque es una operación autocontenida.</summary>
        public void Eliminar(int idAtencion)
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Atencion_Eliminar", conexion))
                {
                    SqlAyudante.Agregar(comando, "@IdAtencion", SqlDbType.Int, idAtencion);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("AtencionRepositorio.Eliminar", ex);
                throw ErroresSql.Traducir(ex, "No fue posible eliminar la atención.");
            }
        }

        #endregion

        #region Apoyo

        private static void InsertarDetalles(SqlConnection conexion, SqlTransaction transaccion,
                                             int idAtencion, IEnumerable<AtencionDetalle> detalles)
        {
            int item = 1;
            foreach (var detalle in detalles)
            {
                using (var comando = SqlAyudante.CrearComando("dbo.usp_AtencionDetalle_Insertar", conexion, transaccion))
                {
                    SqlAyudante.Agregar(comando, "@IdAtencion", SqlDbType.Int, idAtencion);
                    SqlAyudante.Agregar(comando, "@Item", SqlDbType.Int, item);
                    SqlAyudante.Agregar(comando, "@CodigoCie10", SqlDbType.VarChar, detalle.CodigoCie10);
                    SqlAyudante.Agregar(comando, "@DescripcionDiagnostico", SqlDbType.NVarChar, detalle.DescripcionDiagnostico);
                    SqlAyudante.Agregar(comando, "@TipoDiagnostico", SqlDbType.Char, detalle.TipoDiagnostico);
                    SqlAyudante.Agregar(comando, "@Indicaciones", SqlDbType.NVarChar, detalle.Indicaciones);
                    SqlAyudante.Agregar(comando, "@VersionCatalogoCie10", SqlDbType.VarChar, detalle.VersionCatalogoCie10);
                    comando.ExecuteNonQuery();
                }
                item++;
            }
        }

        /// <summary>
        /// Marca como ATENDIDA la cita que originó la atención, dentro de la misma
        /// transacción. Es deliberado que ocurra aquí y no en un paso posterior: si
        /// la atención se guarda pero el cambio de estado falla, la agenda mostraría
        /// como pendiente a alguien que ya fue atendido. O se guardan ambas cosas, o
        /// ninguna.
        /// </summary>
        private static void SincronizarCita(SqlConnection conexion, SqlTransaction transaccion, Atencion atencion)
        {
            if (!atencion.IdCita.HasValue) return;

            string procedimiento = atencion.Estado == "N"
                ? "dbo.usp_Cita_Liberar"      // atención anulada: la cita vuelve a estar pendiente
                : "dbo.usp_Cita_MarcarAtendida";

            using (var comando = SqlAyudante.CrearComando(procedimiento, conexion, transaccion))
            {
                SqlAyudante.Agregar(comando, "@IdCita", SqlDbType.Int, atencion.IdCita);
                comando.ExecuteNonQuery();
            }
        }

        private static void AgregarParametrosCabecera(SqlCommand comando, Atencion atencion)
        {
            SqlAyudante.Agregar(comando, "@IdPaciente", SqlDbType.Int, atencion.IdPaciente);
            SqlAyudante.Agregar(comando, "@IdMedico", SqlDbType.Int, atencion.IdMedico);
            SqlAyudante.Agregar(comando, "@IdCita", SqlDbType.Int, atencion.IdCita);
            SqlAyudante.Agregar(comando, "@FechaAtencion", SqlDbType.DateTime2, atencion.FechaAtencion);
            SqlAyudante.Agregar(comando, "@MotivoConsulta", SqlDbType.NVarChar, atencion.MotivoConsulta);
            SqlAyudante.Agregar(comando, "@Temperatura", SqlDbType.Decimal, atencion.Temperatura);
            SqlAyudante.Agregar(comando, "@PresionArterial", SqlDbType.VarChar, atencion.PresionArterial);
            SqlAyudante.Agregar(comando, "@FrecuenciaCardiaca", SqlDbType.Int, atencion.FrecuenciaCardiaca);
            SqlAyudante.Agregar(comando, "@Peso", SqlDbType.Decimal, atencion.Peso);
            SqlAyudante.Agregar(comando, "@Talla", SqlDbType.Decimal, atencion.Talla);
            SqlAyudante.Agregar(comando, "@Observaciones", SqlDbType.NVarChar, atencion.Observaciones);
            SqlAyudante.Agregar(comando, "@Estado", SqlDbType.Char, atencion.Estado);
        }

        #endregion
    }
}
