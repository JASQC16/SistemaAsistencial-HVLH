using System;
using System.Data;
using Hospital.AccesoDatos;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.Negocio
{
    /// <summary>
    /// Reglas de los reportes de pacientes y atenciones.
    ///
    /// La responsabilidad de esta capa es garantizar que lo que se imprime y lo que
    /// se exporta correspondan exactamente al filtro que el usuario seleccionó: el
    /// RDLC se alimenta de este mismo DataTable, así que no hay forma de que la
    /// exportación muestre algo distinto de lo que se ve en pantalla.
    /// </summary>
    public class ReporteServicio
    {
        private readonly ReporteRepositorio _repositorio = new ReporteRepositorio();

        /// <summary>Tope de amplitud del periodo, para que un rango absurdo no bloquee el servidor.</summary>
        private const int MaximoDias = 1095;   // tres años

        public DataTable Generar(FiltroReporte filtro)
        {
            Validar(filtro);
            return _repositorio.Obtener(filtro);
        }

        private static void Validar(FiltroReporte filtro)
        {
            if (filtro == null) throw new NegocioException("No se recibieron los criterios del reporte.");

            if (filtro.FechaDesde.Date > filtro.FechaHasta.Date)
                throw new NegocioException("La fecha inicial no puede ser posterior a la fecha final.");

            if (filtro.FechaDesde.Date > DateTime.Today)
                throw new NegocioException("La fecha inicial no puede ser futura.");

            int dias = (filtro.FechaHasta.Date - filtro.FechaDesde.Date).Days;
            if (dias > MaximoDias)
                throw new NegocioException(
                    "El periodo consultado no puede superar los tres años. Acote el rango de fechas.");

            if (!string.IsNullOrWhiteSpace(filtro.CodigoCie10) && filtro.CodigoCie10.Trim().Length < 3)
                throw new NegocioException("El código CIE-10 debe tener al menos 3 caracteres (por ejemplo F20).");

            filtro.Documento   = Limpiar(filtro.Documento);
            filtro.CodigoCie10 = Limpiar(filtro.CodigoCie10);
            if (filtro.CodigoCie10 != null) filtro.CodigoCie10 = filtro.CodigoCie10.ToUpperInvariant();
        }

        private static string Limpiar(string texto)
        {
            return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
        }
    }
}
