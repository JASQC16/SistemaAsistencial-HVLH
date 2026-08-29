using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Hospital.Presentacion
{
    /// <summary>
    /// Acceso a los recursos gráficos institucionales.
    ///
    /// Las imágenes van incrustadas en el ensamblado (EmbeddedResource) para que la
    /// aplicación sea autocontenida: no depende de que la carpeta Recursos viaje junto
    /// al ejecutable. Se cargan una sola vez y se conservan en memoria, porque
    /// Image.FromStream en cada formulario multiplicaría el consumo de GDI.
    /// </summary>
    public static class Recursos
    {
        private const string RutaLogo  = "Hospital.Presentacion.Recursos.logo-hvlh.png";
        private const string RutaIcono = "Hospital.Presentacion.Recursos.hvlh.ico";

        private static Image _logo;
        private static Icon _icono;

        /// <summary>Logo oficial del Hospital Nacional Víctor Larco Herrera.</summary>
        public static Image Logo
        {
            get
            {
                if (_logo == null) _logo = CargarImagen(RutaLogo, "logo-hvlh.png");
                return _logo;
            }
        }

        /// <summary>Icono de la aplicación, derivado del logo institucional.</summary>
        public static Icon Icono
        {
            get
            {
                if (_icono == null) _icono = CargarIcono();
                return _icono;
            }
        }

        private static Image CargarImagen(string recurso, string nombreArchivo)
        {
            try
            {
                using (Stream flujo = Assembly.GetExecutingAssembly().GetManifestResourceStream(recurso))
                {
                    if (flujo != null) return Image.FromStream(flujo);
                }

                // Respaldo: archivo copiado a la carpeta de salida.
                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos", nombreArchivo);
                if (File.Exists(ruta)) return Image.FromFile(ruta);
            }
            catch (Exception)
            {
                // Un problema con el logo nunca debe impedir el uso del sistema:
                // los formularios simplemente se muestran sin imagen.
            }
            return null;
        }

        private static Icon CargarIcono()
        {
            try
            {
                using (Stream flujo = Assembly.GetExecutingAssembly().GetManifestResourceStream(RutaIcono))
                {
                    if (flujo != null) return new Icon(flujo);
                }

                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos", "hvlh.ico");
                if (File.Exists(ruta)) return new Icon(ruta);
            }
            catch (Exception)
            {
            }
            return null;
        }
    }
}
