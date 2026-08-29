using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Hospital.Entidades;
using Hospital.Utilidades;
using Newtonsoft.Json.Linq;

namespace Hospital.Integracion
{
    /// <summary>
    /// Consumo de la API REST pública de la U.S. National Library of Medicine
    /// (Clinical Table Search Service) para buscar diagnósticos CIE-10.
    /// Es de acceso libre y no requiere API key.
    ///
    /// Endpoint:
    ///   https://clinicaltables.nlm.nih.gov/api/icd10cm/v3/search?sf=code,name&amp;terms=diabetes&amp;maxList=20
    ///
    /// Respuesta (arreglo heterogéneo):
    ///   [ 59, ["A00","A000"], null, [["A00","Cholera"], ["A000","Cholera due to ..."]] ]
    ///   Posición 0: total de coincidencias
    ///   Posición 3: pares [código, descripción]
    /// </summary>
    public class Cie10ApiCliente
    {
        /// <summary>
        /// HttpClient se declara estático y se reutiliza durante toda la vida de la
        /// aplicación. Crear una instancia por llamada agota los sockets disponibles
        /// (socket exhaustion) porque las conexiones quedan en TIME_WAIT.
        /// </summary>
        private static readonly HttpClient Cliente = ConstruirCliente();

        private static HttpClient ConstruirCliente()
        {
            // TLS 1.2: .NET Framework 4.7 puede negociar por defecto un protocolo
            // que el servicio ya no acepta.
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var cliente = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            cliente.DefaultRequestHeaders.Add("Accept", "application/json");
            cliente.DefaultRequestHeaders.Add("User-Agent", "HospitalAtenciones/1.0");
            return cliente;
        }

        private static string UrlBase
        {
            get
            {
                string url = ConfigurationManager.AppSettings["Cie10ApiUrl"];
                return string.IsNullOrWhiteSpace(url)
                    ? "https://clinicaltables.nlm.nih.gov/api/icd10cm/v3/search"
                    : url;
            }
        }

        /// <summary>
        /// Busca diagnósticos por término. Es asíncrono de extremo a extremo para no
        /// bloquear el hilo de interfaz mientras se espera la respuesta de la red.
        /// </summary>
        public async Task<List<DiagnosticoCie10>> BuscarAsync(string termino, CancellationToken cancelacion)
        {
            var resultados = new List<DiagnosticoCie10>();

            if (string.IsNullOrWhiteSpace(termino) || termino.Trim().Length < 2)
                return resultados;

            string url = string.Format("{0}?sf=code,name&maxList=25&terms={1}",
                                       UrlBase, Uri.EscapeDataString(termino.Trim()));

            try
            {
                using (HttpResponseMessage respuesta = await Cliente.GetAsync(url, cancelacion).ConfigureAwait(false))
                {
                    if (!respuesta.IsSuccessStatusCode)
                    {
                        throw new NegocioException(
                            "El servicio de catálogo CIE-10 respondió con el código " +
                            (int)respuesta.StatusCode + " (" + respuesta.ReasonPhrase + ").");
                    }

                    string json = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return Deserializar(json);
                }
            }
            catch (OperationCanceledException)
            {
                // Búsqueda cancelada por el usuario al seguir escribiendo: no es un error.
                return resultados;
            }
            catch (HttpRequestException ex)
            {
                Registro.Error("Cie10ApiCliente.BuscarAsync", ex);
                throw new NegocioException(
                    "No se pudo contactar el catálogo CIE-10. Verifique su conexión a internet " +
                    "o ingrese el diagnóstico manualmente.", ex);
            }
            catch (Exception ex)
            {
                Registro.Error("Cie10ApiCliente.BuscarAsync", ex);
                throw new NegocioException("Ocurrió un problema al consultar el catálogo CIE-10.", ex);
            }
        }

        /// <summary>
        /// Deserialización del arreglo JSON. Se usa el modelo dinámico de Json.NET
        /// (JArray) porque la respuesta no es un objeto con propiedades fijas sino un
        /// arreglo posicional heterogéneo, que no mapea contra una clase POCO.
        /// </summary>
        private static List<DiagnosticoCie10> Deserializar(string json)
        {
            var resultados = new List<DiagnosticoCie10>();
            if (string.IsNullOrWhiteSpace(json)) return resultados;

            JArray raiz = JArray.Parse(json);
            if (raiz.Count < 4) return resultados;

            var filas = raiz[3] as JArray;
            if (filas == null) return resultados;

            foreach (JToken fila in filas)
            {
                var par = fila as JArray;
                if (par == null || par.Count < 2) continue;

                resultados.Add(new DiagnosticoCie10
                {
                    Codigo      = par[0].ToString(),
                    Descripcion = par[1].ToString()
                });
            }
            return resultados;
        }
    }
}
