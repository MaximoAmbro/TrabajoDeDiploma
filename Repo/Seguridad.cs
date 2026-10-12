using System;
using System.Security.Cryptography;
using System.Text;

namespace Negocio
{
    // Hash de contraseñas con PBKDF2 (HMAC-SHA256) y salt aleatorio por usuario.
    // Formato almacenado: PBKDF2$iteraciones$saltBase64$hashBase64 (83 caracteres, entra en VARCHAR(100)).
    // Las iteraciones viajan en el valor guardado, así se pueden subir sin invalidar hashes viejos.
    public static class Seguridad
    {
        private const string Prefijo = "PBKDF2";
        private const char Separador = '$';
        private const int Iteraciones = 100000;
        private const int BytesSalt = 16;
        private const int BytesHash = 32;

        public static string Hash(string contrasena)
        {
            byte[] salt = new byte[BytesSalt];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            byte[] hash = Derivar(contrasena, salt, Iteraciones, BytesHash);

            return string.Join(Separador.ToString(),
                Prefijo, Iteraciones.ToString(), Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public static bool Verificar(string contrasena, string almacenado)
        {
            if (contrasena == null || !EsHash(almacenado))
            {
                return false;
            }

            string[] partes = almacenado.Split(Separador);
            try
            {
                int iteraciones = int.Parse(partes[1]);
                byte[] salt = Convert.FromBase64String(partes[2]);
                byte[] esperado = Convert.FromBase64String(partes[3]);
                byte[] calculado = Derivar(contrasena, salt, iteraciones, esperado.Length);
                return SonIguales(esperado, calculado);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // Distingue un hash de una contraseña heredada en texto plano.
        public static bool EsHash(string almacenado)
        {
            if (string.IsNullOrEmpty(almacenado))
            {
                return false;
            }
            string[] partes = almacenado.Split(Separador);
            return partes.Length == 4 && partes[0] == Prefijo;
        }

        // PBKDF2 según RFC 2898. Se implementa a mano porque en netstandard2.0 Rfc2898DeriveBytes
        // solo ofrece HMAC-SHA1; el constructor con HashAlgorithmName no existe.
        private static byte[] Derivar(string contrasena, byte[] salt, int iteraciones, int bytes)
        {
            byte[] resultado = new byte[bytes];
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(contrasena)))
            {
                int tamBloque = hmac.HashSize / 8;
                int bloques = (bytes + tamBloque - 1) / tamBloque;
                byte[] entrada = new byte[salt.Length + 4];
                Buffer.BlockCopy(salt, 0, entrada, 0, salt.Length);

                for (int bloque = 1; bloque <= bloques; bloque++)
                {
                    // U1 = HMAC(P, S || INT_32_BE(bloque))
                    entrada[salt.Length] = (byte)(bloque >> 24);
                    entrada[salt.Length + 1] = (byte)(bloque >> 16);
                    entrada[salt.Length + 2] = (byte)(bloque >> 8);
                    entrada[salt.Length + 3] = (byte)bloque;

                    byte[] u = hmac.ComputeHash(entrada);
                    byte[] t = (byte[])u.Clone();
                    for (int i = 1; i < iteraciones; i++)
                    {
                        u = hmac.ComputeHash(u);
                        for (int j = 0; j < t.Length; j++)
                        {
                            t[j] ^= u[j];
                        }
                    }

                    int desplazamiento = (bloque - 1) * tamBloque;
                    Buffer.BlockCopy(t, 0, resultado, desplazamiento, Math.Min(tamBloque, bytes - desplazamiento));
                }
            }
            return resultado;
        }

        // Comparación en tiempo constante: no corta en el primer byte distinto.
        private static bool SonIguales(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            int diferencia = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diferencia |= a[i] ^ b[i];
            }
            return diferencia == 0;
        }
    }
}
