using System;
using System.Data;
using System.Data.SqlClient;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.AccesoDatos
{
    /// <summary>
    /// Fuente de datos de los reportes.
    ///
    /// Devuelve un DataTable en lugar de una lista de entidades porque el destino es
    /// ReportViewer, que se enlaza a tablas: convertir a objetos y de vuelta a tabla
    /// solo añadiría trabajo sin aportar nada.
    /// </summary>
    public class ReporteRepositorio
    {
        public DataTable Obtener(FiltroReporte filtro)
        {
            if (filtro == null) throw new ArgumentNullException("filtro");

            try
            {
                var tabla = new DataTable("Reporte");

                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Reporte_General", conexion))
                {
                    SqlAyudante.Agregar(comando, "@FechaDesde", SqlDbType.Date, filtro.FechaDesde.Date);
                    SqlAyudante.Agregar(comando, "@FechaHasta", SqlDbType.Date, filtro.FechaHasta.Date);
                    SqlAyudante.Agregar(comando, "@Estado", SqlDbType.VarChar, filtro.Estado);
                    SqlAyudante.Agregar(comando, "@Documento", SqlDbType.VarChar, filtro.Documento);
                    SqlAyudante.Agregar(comando, "@IdMedico", SqlDbType.Int, filtro.IdMedico);
                    SqlAyudante.Agregar(comando, "@IdEspecialidad", SqlDbType.Int, filtro.IdEspecialidad);
                    SqlAyudante.Agregar(comando, "@CodigoCie10", SqlDbType.VarChar, filtro.CodigoCie10);

                    using (var adaptador = new SqlDataAdapter(comando))
                    {
                        adaptador.Fill(tabla);
                    }
                }

                return tabla;
            }
            catch (SqlException ex)
            {
                Registro.Error("ReporteRepositorio.Obtener", ex);
                throw ErroresSql.Traducir(ex, "No fue posible generar la información del reporte.");
            }
        }
    }
}
