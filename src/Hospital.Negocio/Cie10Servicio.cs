using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hospital.AccesoDatos;
using Hospital.Entidades;
using Hospital.Integracion;
using Hospital.Utilidades;

namespace Hospital.Negocio
{
    /// <summary>
    /// Fachada del catálogo CIE-10.
    ///
    /// La fuente principal es el catálogo oficial del MINSA almacenado en SQL Server
    /// (dbo.CatalogoCie10), en español: responde en milisegundos, funciona sin
    /// conexión y es la codificación que el hospital debe reportar al HIS.
    ///
    /// La API REST pública se conserva como fuente complementaria y solo entra en
    /// juego cuando la búsqueda local no devuelve nada, para poder consultar códigos
    /// que aún no estén en la carga local. Sus descripciones vienen en inglés, así
    /// que se marcan como referencia externa y no sustituyen al catálogo oficial.
    ///
    /// La presentación depende de este servicio y no del origen concreto: cambiar de
    /// proveedor de datos no obliga a tocar los formularios.
    /// </summary>
    public class Cie10Servicio
    {
        private readonly Cie10Repositorio _repositorio = new Cie10Repositorio();
        private readonly Cie10ApiCliente _cliente = new Cie10ApiCliente();

        private const int MaximoResultados = 50;

        /// <summary>Versión del catálogo, leída una sola vez por sesión.</summary>
        private static string _version;
        private static bool _versionLeida;

        /// <summary>
        /// Búsqueda contra el catálogo local. Acepta indistintamente un código
        /// ("F20", "F20.0") o un texto ("esquizofrenia"), y admite coincidencias
        /// parciales en ambos casos.
        /// </summary>
        public List<DiagnosticoCie10> Buscar(string termino)
        {
            ValidarTermino(termino);
            return _repositorio.Buscar(termino.Trim(), soloVigentes: true, maximo: MaximoResultados);
        }

        /// <summary>
        /// Búsqueda asíncrona usada por el formulario de atenciones: consulta primero
        /// el catálogo local y, solo si no encuentra nada, recurre a la API externa.
        /// La llamada a la base va en un hilo del pool para no bloquear la interfaz
        /// mientras se escribe.
        /// </summary>
        public async Task<List<DiagnosticoCie10>> BuscarAsync(string termino, CancellationToken cancelacion)
        {
            ValidarTermino(termino);
            string texto = termino.Trim();

            var locales = await Task.Run(
                () => _repositorio.Buscar(texto, true, MaximoResultados), cancelacion).ConfigureAwait(false);

            if (locales.Count > 0) return locales;

            cancelacion.ThrowIfCancellationRequested();

            // Respaldo externo: nunca debe impedir el registro de la atención, así que
            // un fallo de red aquí se traduce en "sin resultados", no en una excepción.
            try
            {
                var externos = await _cliente.BuscarAsync(texto, cancelacion).ConfigureAwait(false);
                foreach (var diagnostico in externos) diagnostico.VersionCatalogo = "REFERENCIA-EXTERNA";
                return externos;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Registro.Error("Cie10Servicio.BuscarAsync (respaldo externo)", ex);
                return new List<DiagnosticoCie10>();
            }
        }

        /// <summary>Verifica que un código escrito a mano exista realmente en el catálogo.</summary>
        public DiagnosticoCie10 ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new NegocioException("Ingrese un código CIE-10.");

            return _repositorio.ObtenerPorCodigo(codigo.Trim().ToUpperInvariant());
        }

        /// <summary>
        /// Versión del catálogo vigente. Se guarda en cada diagnóstico registrado para
        /// dejar constancia de con qué edición del catálogo del MINSA se codificó la
        /// atención.
        /// </summary>
        public string VersionCatalogo()
        {
            if (!_versionLeida)
            {
                _version = _repositorio.ObtenerVersionVigente();
                _versionLeida = true;
            }
            return _version;
        }

        private static void ValidarTermino(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino) || termino.Trim().Length < 2)
                throw new NegocioException("Ingrese al menos 2 caracteres para buscar un diagnóstico.");
        }
    }
}
