using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hospital.AccesoDatos;
using Hospital.Entidades;
using Hospital.Utilidades;

namespace Hospital.Negocio
{
    /// <summary>
    /// Reglas de negocio del proceso asistencial. La capa de presentación no valida
    /// nada por su cuenta: siempre pasa por aquí, de modo que las mismas reglas
    /// aplicarían si mañana el consumidor fuera una API web en lugar de un WinForms.
    /// </summary>
    public class AtencionServicio
    {
        private readonly AtencionRepositorio _repositorio = new AtencionRepositorio();
        private readonly Cie10Servicio _cie10 = new Cie10Servicio();

        public List<AtencionResumen> Listar(FiltroAtencion filtro)
        {
            if (filtro == null) filtro = new FiltroAtencion();

            if (filtro.FechaDesde.HasValue && filtro.FechaHasta.HasValue &&
                filtro.FechaDesde.Value.Date > filtro.FechaHasta.Value.Date)
            {
                throw new NegocioException("La fecha inicial no puede ser posterior a la fecha final.");
            }

            return _repositorio.Listar(filtro);
        }

        public Atencion ObtenerPorId(int idAtencion)
        {
            var atencion = _repositorio.ObtenerPorId(idAtencion);
            if (atencion == null)
                throw new NegocioException("La atención ya no existe. Actualice la consulta.");
            return atencion;
        }

        public int Registrar(Atencion atencion)
        {
            Validar(atencion);
            atencion.IdUsuarioRegistro = Sesion.IdUsuario;
            SellarVersionCatalogo(atencion);
            return _repositorio.Insertar(atencion);
        }

        public void Actualizar(Atencion atencion)
        {
            if (atencion.IdAtencion <= 0)
                throw new NegocioException("No se ha identificado la atención a modificar.");

            Validar(atencion);

            if (atencion.Estado == "N")
                throw new NegocioException("Una atención anulada no puede modificarse.");

            SellarVersionCatalogo(atencion);
            _repositorio.Actualizar(atencion);
        }

        /// <summary>
        /// Deja constancia de con qué versión del catálogo CIE-10 se codificó cada
        /// diagnóstico. Junto con la copia del código y de la descripción que guarda
        /// el detalle, esto es lo que impide que una actualización futura del catálogo
        /// del MINSA reescriba diagnósticos ya asentados en la historia clínica.
        /// </summary>
        private void SellarVersionCatalogo(Atencion atencion)
        {
            string version = _cie10.VersionCatalogo();
            if (string.IsNullOrEmpty(version)) return;

            foreach (var detalle in atencion.Detalles)
            {
                if (string.IsNullOrEmpty(detalle.VersionCatalogoCie10))
                    detalle.VersionCatalogoCie10 = version;
            }
        }

        public void Anular(int idAtencion, string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new NegocioException("Indique el motivo de la anulación.");
            if (motivo.Trim().Length < 5)
                throw new NegocioException("El motivo de anulación debe tener al menos 5 caracteres.");

            _repositorio.Anular(idAtencion, motivo.Trim());
        }

        public void Eliminar(int idAtencion)
        {
            if (!Sesion.EsAdministrador)
                throw new NegocioException("Solo un usuario administrador puede eliminar atenciones de forma definitiva.");

            _repositorio.Eliminar(idAtencion);
        }

        /// <summary>
        /// Valida campos obligatorios y reglas de negocio. Acumula todos los errores en
        /// un solo mensaje para que la usuaria corrija de una vez y no de uno en uno.
        /// </summary>
        private static void Validar(Atencion atencion)
        {
            if (atencion == null) throw new NegocioException("No hay datos para guardar.");

            var errores = new List<string>();

            // --- Campos obligatorios de la cabecera ---
            if (atencion.IdPaciente <= 0)  errores.Add("Debe seleccionar un paciente.");
            if (atencion.IdMedico <= 0)    errores.Add("Debe seleccionar un médico responsable.");
            if (string.IsNullOrWhiteSpace(atencion.MotivoConsulta))
                errores.Add("El motivo de consulta es obligatorio.");
            else if (atencion.MotivoConsulta.Trim().Length < 5)
                errores.Add("El motivo de consulta debe tener al menos 5 caracteres.");

            // --- Reglas sobre la fecha ---
            if (atencion.FechaAtencion == DateTime.MinValue)
                errores.Add("La fecha de atención es obligatoria.");
            else if (atencion.FechaAtencion > DateTime.Now.AddMinutes(5))
                errores.Add("La fecha de atención no puede ser futura.");
            else if (atencion.FechaAtencion < DateTime.Now.AddYears(-5))
                errores.Add("La fecha de atención no puede tener más de 5 años de antigüedad.");

            // --- Rangos clínicos de los signos vitales ---
            if (atencion.Temperatura.HasValue && (atencion.Temperatura < 30m || atencion.Temperatura > 45m))
                errores.Add("La temperatura debe estar entre 30 °C y 45 °C.");

            if (atencion.FrecuenciaCardiaca.HasValue &&
                (atencion.FrecuenciaCardiaca < 20 || atencion.FrecuenciaCardiaca > 250))
                errores.Add("La frecuencia cardiaca debe estar entre 20 y 250 lpm.");

            if (atencion.Peso.HasValue && (atencion.Peso <= 0m || atencion.Peso > 400m))
                errores.Add("El peso debe ser mayor a 0 kg y menor a 400 kg.");

            if (atencion.Talla.HasValue && (atencion.Talla <= 0m || atencion.Talla > 2.5m))
                errores.Add("La talla debe expresarse en metros y ser menor a 2.50 m.");

            if (!string.IsNullOrWhiteSpace(atencion.PresionArterial) &&
                !System.Text.RegularExpressions.Regex.IsMatch(atencion.PresionArterial, @"^\d{2,3}/\d{2,3}$"))
                errores.Add("La presión arterial debe tener el formato sistólica/diastólica, por ejemplo 120/80.");

            // --- Reglas del detalle ---
            if (atencion.Detalles == null || atencion.Detalles.Count == 0)
            {
                errores.Add("Registre al menos un diagnóstico en el detalle de la atención.");
            }
            else
            {
                if (atencion.Detalles.Count > 20)
                    errores.Add("No se pueden registrar más de 20 diagnósticos en una misma atención.");

                if (atencion.Detalles.Any(d => string.IsNullOrWhiteSpace(d.CodigoCie10) ||
                                               string.IsNullOrWhiteSpace(d.DescripcionDiagnostico)))
                    errores.Add("Todo diagnóstico debe tener código CIE-10 y descripción.");

                if (atencion.Detalles.Any(d => string.IsNullOrWhiteSpace(d.TipoDiagnostico)))
                    errores.Add("Indique el tipo de cada diagnóstico (presuntivo, definitivo o repetitivo).");

                var duplicados = atencion.Detalles
                    .Where(d => !string.IsNullOrWhiteSpace(d.CodigoCie10))
                    .GroupBy(d => d.CodigoCie10.Trim().ToUpperInvariant())
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicados.Count > 0)
                    errores.Add("El diagnóstico " + string.Join(", ", duplicados) +
                                " está repetido en el detalle.");

                // Una atención cerrada exige un diagnóstico definitivo.
                if (atencion.Estado == "A" && !atencion.Detalles.Any(d => d.TipoDiagnostico == "D"))
                    errores.Add("Para marcar la atención como Atendida debe existir al menos un diagnóstico definitivo.");
            }

            if (errores.Count > 0)
            {
                var mensaje = new StringBuilder("Corrija los siguientes puntos antes de guardar:").AppendLine().AppendLine();
                foreach (var error in errores) mensaje.Append("   •  ").AppendLine(error);
                throw new NegocioException(mensaje.ToString().TrimEnd());
            }
        }
    }
}
