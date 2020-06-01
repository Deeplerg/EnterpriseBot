using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace EnterpriseBot.Api.Utils
{
    public static class Hash
    {
        private const int saltSize = 128 / 8;
        private const int hashSize = 256 / 8;
        private const int iterations = 10000;

        public static byte[] CreateSalt()
        {
            byte[] salt = new byte[saltSize];
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
                iterationCount: iterations,
                numBytesRequested: hashSize));
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

        //public static string GenerateSHA512(string input)
        //{
        //    byte[] hash;
        //    using(var sha512 = new SHA512Managed())
        //    {
        //        hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
        //    }

        //    StringBuilder sb = new StringBuilder();
        //    foreach(var b in hash)
        //        sb.Append(b.ToString("X2"));
        //    return sb.ToString();
        //}

        //public static string VerifySHA512(string hash, string input)
        //{

        //}
    }
}
