using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.AccesoDatos
{
    public class MaestroRepositorio
    {
        public List<Medico> ListarMedicos()
        {
            var lista = new List<Medico>();
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Medico_Listar", conexion))
                {
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Medico
                            {
                                IdMedico          = SqlAyudante.LeerEntero(lector, "IdMedico"),
                                NumeroColegiatura = SqlAyudante.LeerTexto(lector, "NumeroColegiatura"),
                                Nombres           = SqlAyudante.LeerTexto(lector, "Nombres"),
                                Apellidos         = SqlAyudante.LeerTexto(lector, "Apellidos"),
                                IdEspecialidad    = SqlAyudante.LeerEntero(lector, "IdEspecialidad"),
                                Especialidad      = SqlAyudante.LeerTexto(lector, "Especialidad")
                            });
                        }
                    }
                }
                return lista;
            }
            catch (SqlException ex)
            {
                Registro.Error("MaestroRepositorio.ListarMedicos", ex);
                throw ErroresSql.Traducir(ex, "No fue posible obtener la lista de médicos.");
            }
        }

        /// <summary>Servicios / especialidades activas, para los combos de filtro.</summary>
        public List<Especialidad> ListarEspecialidades()
        {
            var lista = new List<Especialidad>();
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Especialidad_Listar", conexion))
                {
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Especialidad
                            {
                                IdEspecialidad = SqlAyudante.LeerEntero(lector, "IdEspecialidad"),
                                Nombre         = SqlAyudante.LeerTexto(lector, "Nombre")
                            });
                        }
                    }
                }
                return lista;
            }
            catch (SqlException ex)
            {
                Registro.Error("MaestroRepositorio.ListarEspecialidades", ex);
                throw ErroresSql.Traducir(ex, "No fue posible obtener la lista de servicios.");
            }
        }

    }
}
