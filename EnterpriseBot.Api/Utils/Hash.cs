using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace EnterpriseBot.Api.Utils
{
    public static class Hash
    {
        private const int SaltSize = 128 / 8;
        private const int HashSize = 256 / 8;
        private const int Iterations = 10000;

        public static byte[] CreateSalt()
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static string CreateHash(string input, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: input,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: Iterations,
                numBytesRequested: HashSize));
        }

        public static bool Verify(string input, byte[] salt, string hash)
        {
            return string.Equals(CreateHash(input, salt), hash);
        }

        public static void VerifyAndThrow(string input, byte[] salt, string hash)
        {
            if (!string.Equals(CreateHash(input, salt), hash))
                throw new ArgumentException($"{nameof(input)} has not passed hash verification");
        }
    }
}
