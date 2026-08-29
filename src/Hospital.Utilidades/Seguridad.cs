using System;
using System.Security.Cryptography;

namespace Hospital.Utilidades
{
    /// <summary>
    /// Derivación de claves con PBKDF2 (Rfc2898). No se usa MD5/SHA1 plano porque
    /// son demasiado rápidos de calcular y facilitan ataques de fuerza bruta.
    /// </summary>
    public static class Seguridad
    {
        private const int Iteraciones = 20000;
        private const int TamanioHash = 32;
        private const int TamanioSalt = 16;

        public static string GenerarSalt()
        {
            byte[] salt = new byte[TamanioSalt];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        public static string CalcularHash(string clave, string saltBase64)
        {
            if (string.IsNullOrEmpty(clave)) throw new ArgumentException("La clave no puede estar vacía.", "clave");

            byte[] salt = Convert.FromBase64String(saltBase64);
            using (var derivador = new Rfc2898DeriveBytes(clave, salt, Iteraciones, HashAlgorithmName.SHA256))
            {
                return Convert.ToBase64String(derivador.GetBytes(TamanioHash));
            }
        }

        /// <summary>Comparación en tiempo constante para no filtrar información por el tiempo de respuesta.</summary>
        public static bool VerificarClave(string clave, string saltBase64, string hashEsperado)
        {
            if (string.IsNullOrEmpty(saltBase64) || string.IsNullOrEmpty(hashEsperado)) return false;

            string hashCalculado = CalcularHash(clave, saltBase64);
            byte[] a = Convert.FromBase64String(hashCalculado);
            byte[] b = Convert.FromBase64String(hashEsperado);
            if (a.Length != b.Length) return false;

            int diferencia = 0;
            for (int i = 0; i < a.Length; i++) diferencia |= a[i] ^ b[i];
            return diferencia == 0;
        }
    }
}
