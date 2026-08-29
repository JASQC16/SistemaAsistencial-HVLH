using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.AccesoDatos
{
    /// <summary>
    /// Consulta del catálogo CIE-10 oficial del MINSA almacenado en SQL Server.
    ///
    /// Antes esta búsqueda se resolvía llamando a una API pública estadounidense, lo
    /// que devolvía las descripciones en inglés y dejaba el registro clínico a merced
    /// de la conexión a Internet. Ahora el catálogo vive en dbo.CatalogoCie10 y se
    /// alimenta del archivo oficial del MINSA, en español.
    /// </summary>
    public class Cie10Repositorio
    {
        /// <summary>
        /// Busca por código o por texto en una sola operación: el procedimiento decide
        /// cuál de las dos interpretaciones aplica y ordena los resultados poniendo
        /// primero las coincidencias exactas y por prefijo.
        /// </summary>
        public List<DiagnosticoCie10> Buscar(string termino, bool soloVigentes, int maximo)
        {
            var lista = new List<DiagnosticoCie10>();
            if (string.IsNullOrWhiteSpace(termino)) return lista;

            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Cie10_Buscar", conexion))
                {
                    SqlAyudante.Agregar(comando, "@Termino", SqlDbType.NVarChar, termino.Trim());
                    SqlAyudante.Agregar(comando, "@SoloVigentes", SqlDbType.Bit, soloVigentes);
                    SqlAyudante.Agregar(comando, "@Maximo", SqlDbType.Int, maximo);

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
                Registro.Error("Cie10Repositorio.Buscar", ex);
                throw ErroresSql.Traducir(ex, "No fue posible consultar el catálogo CIE-10.");
            }
        }

        public DiagnosticoCie10 ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;

            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Cie10_ObtenerPorCodigo", conexion))
                {
                    SqlAyudante.Agregar(comando, "@CodigoCie10", SqlDbType.VarChar, codigo.Trim());
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        return lector.Read() ? Mapear(lector) : null;
                    }
                }
            }
            catch (SqlException ex)
            {
                Registro.Error("Cie10Repositorio.ObtenerPorCodigo", ex);
                throw ErroresSql.Traducir(ex, "No fue posible verificar el código CIE-10.");
            }
        }

        /// <summary>
        /// Versión del catálogo cargada actualmente. Se guarda junto con cada
        /// diagnóstico registrado, para saber con qué edición del catálogo se
        /// codificó cada atención.
        /// </summary>
        public string ObtenerVersionVigente()
        {
            try
            {
                using (var conexion = ConexionBD.Crear())
                using (var comando = SqlAyudante.CrearComando("dbo.usp_Cie10_ObtenerVersion", conexion))
                {
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        return lector.Read() ? SqlAyudante.LeerTexto(lector, "VersionCatalogo") : null;
                    }
                }
            }
            catch (SqlException ex)
            {
                // No poder leer la versión no debe impedir registrar la atención.
                Registro.Error("Cie10Repositorio.ObtenerVersionVigente", ex);
                return null;
            }
        }

        private static DiagnosticoCie10 Mapear(IDataRecord registro)
        {
            return new DiagnosticoCie10
            {
                Codigo          = SqlAyudante.LeerTexto(registro, "CodigoCie10"),
                CodigoFormato   = SqlAyudante.LeerTexto(registro, "CodigoFormato"),
                Descripcion     = SqlAyudante.LeerTexto(registro, "Descripcion"),
                Categoria       = SqlAyudante.LeerTexto(registro, "Categoria"),
                Grupo           = SqlAyudante.LeerTexto(registro, "Grupo"),
                Capitulo        = SqlAyudante.LeerTexto(registro, "Capitulo"),
                CapituloNombre  = SqlAyudante.LeerTexto(registro, "CapituloNombre"),
                Sexo            = SqlAyudante.LeerTexto(registro, "Sexo"),
                Estado          = SqlAyudante.LeerTexto(registro, "Estado"),
                VersionCatalogo = SqlAyudante.LeerTexto(registro, "VersionCatalogo")
            };
        }
    }
}
