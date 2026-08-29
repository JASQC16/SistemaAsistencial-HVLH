using System;

namespace Hospital.Utilidades
{
    /// <summary>
    /// Error de validación o de regla de negocio. Su mensaje está redactado para el
    /// usuario final y la interfaz lo muestra tal cual.
    /// </summary>
    [Serializable]
    public class NegocioException : Exception
    {
        public NegocioException(string mensaje) : base(mensaje) { }
        public NegocioException(string mensaje, Exception interna) : base(mensaje, interna) { }
    }

    /// <summary>
    /// Error de infraestructura (base de datos, red). Envuelve la excepción técnica
    /// para que no llegue a la pantalla, pero la conserva para el archivo de log.
    /// </summary>
    [Serializable]
    public class DatosException : Exception
    {
        public DatosException(string mensaje, Exception interna) : base(mensaje, interna) { }
    }
}
