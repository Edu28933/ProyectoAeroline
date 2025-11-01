using System;
using System.Linq;
using System.Security.Cryptography;

namespace ProyectoAeroline.Seguridad
{
    public static class PasswordHasher
    {
        // Devuelve Base64 de [salt(16) | hash(32)]
        public static string Hash(string password, int iterations = 100_000)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password vacío", nameof(password));

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            var payload = new byte[1 + 4 + salt.Length + hash.Length];
            payload[0] = 1; // versión
            BitConverter.GetBytes(iterations).CopyTo(payload, 1);
            Buffer.BlockCopy(salt, 0, payload, 5, salt.Length);
            Buffer.BlockCopy(hash, 0, payload, 5 + salt.Length, hash.Length);

            return Convert.ToBase64String(payload);
        }

        public static bool Verify(string password, string encoded)
        {
            if (string.IsNullOrWhiteSpace(encoded)) return false;

            var payload = Convert.FromBase64String(encoded);
            var version = payload[0];
            if (version != 1) return false;

            var iterations = BitConverter.ToInt32(payload, 1);
            var salt = payload.Skip(5).Take(16).ToArray();
            var hash = payload.Skip(21).Take(32).ToArray();

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var test = pbkdf2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(hash, test);
        }
    }
}
