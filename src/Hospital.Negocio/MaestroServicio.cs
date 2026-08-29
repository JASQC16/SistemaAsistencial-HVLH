using System.Collections.Generic;
using Hospital.AccesoDatos;
using Hospital.Entidades;

namespace Hospital.Negocio
{
    /// <summary>Consulta de los maestros que alimentan los combos de la aplicación.</summary>
    public class MaestroServicio
    {
        private readonly MaestroRepositorio _repositorio = new MaestroRepositorio();
        private readonly PacienteRepositorio _pacientes = new PacienteRepositorio();

        public List<Medico> ListarMedicos()
        {
            return _repositorio.ListarMedicos();
        }

        public List<Especialidad> ListarEspecialidades()
        {
            return _repositorio.ListarEspecialidades();
        }

        /// <summary>
        /// Búsqueda rápida de pacientes para los selectores. Se delega en el
        /// repositorio de pacientes para que exista una sola definición de qué es
        /// "buscar un paciente" en todo el sistema.
        /// </summary>
        public List<Paciente> BuscarPacientes(string busqueda)
        {
            return _pacientes.Buscar(string.IsNullOrWhiteSpace(busqueda) ? null : busqueda.Trim());
        }
    }
}
