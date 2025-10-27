using System;
using System.Security.Cryptography;

namespace ProyectoAeroline.Seguridad
{
    public interface IPasswordService
    {
        string Hash(string plain);
        bool Verify(string plain, string stored);
    }

    // Soporta:
    // - Formato PBKDF2:  iteraciones.saltBase64.hashBase64
    // - Texto plano heredado (comparación directa)
    public class PasswordService : IPasswordService
    {
        private const int Iter = 100_000;
        private const int Salt = 16; // 128-bit
        private const int Key = 32; // 256-bit

        public string Hash(string plain)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[Salt];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(plain, salt, Iter, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(Key);
            return $"{Iter}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public bool Verify(string plain, string stored)
        {
            if (string.IsNullOrWhiteSpace(stored)) return false;

            var parts = stored.Split('.');
            if (parts.Length == 3 && int.TryParse(parts[0], out var iters))
            {
                var salt = Convert.FromBase64String(parts[1]);
                var hash = Convert.FromBase64String(parts[2]);

                using var pbkdf2 = new Rfc2898DeriveBytes(plain, salt, iters, HashAlgorithmName.SHA256);
                var candidate = pbkdf2.GetBytes(hash.Length);

                return CryptographicOperations.FixedTimeEquals(candidate, hash);
            }

            // Fallback: contraseña en texto plano (actual estado de muchas BD)
            return string.Equals(plain, stored);
        }
    }
}
