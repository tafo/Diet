using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Diet.Api.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int IterationCount = 10000;

        public string HashPassword(string password)
        {
            var randomNumberGenerator = RandomNumberGenerator.Create();
            const int saltSize = 128 / 8;
            const int numBytesRequested = 256 / 8;
            const KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256;
            var salt = new byte[saltSize];
            randomNumberGenerator.GetBytes(salt);
            var subKey = KeyDerivation.Pbkdf2(password, salt, prf, IterationCount, numBytesRequested);
            var outputBytes = new byte[9 + salt.Length + subKey.Length];
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 9, salt.Length);
            Buffer.BlockCopy(subKey, 0, outputBytes, 9 + saltSize, subKey.Length);
            return Convert.ToBase64String(outputBytes);
        }

        public bool VerifyHashedPassword(string providedPassword, string hashedPassword)
        {
            var decodedHashedPassword = Convert.FromBase64String(hashedPassword);
            if (decodedHashedPassword.Length == 0)
            {
                return false;
            }

            try
            {
                // Read header information
                var prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);
                var saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 5);

                // Read the salt: must be 128 bits
                if (saltLength < 128 / 8)
                {
                    return false;
                }

                var salt = new byte[saltLength];
                Buffer.BlockCopy(decodedHashedPassword, 9, salt, 0, salt.Length);

                // Read the subKey (the rest of the payload): must be >= 128 bits
                var subKeyLength = decodedHashedPassword.Length - 9 - salt.Length;
                if (subKeyLength < 128 / 8)
                {
                    return false;
                }

                var expectedSubKey = new byte[subKeyLength];
                Buffer.BlockCopy(decodedHashedPassword, 9 + salt.Length, expectedSubKey, 0, expectedSubKey.Length);

                // Hash the incoming password and verify it
                var actualSubKey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, IterationCount, subKeyLength);

                return ByteArraysEqual(actualSubKey, expectedSubKey);
            }
            catch
            {
                // ToDo : Log this
                return false;
            }
        }

        private static bool ByteArraysEqual(IReadOnlyList<byte> first, IReadOnlyList<byte> second)
        {
            if (first == null && second == null)
            {
                return true;
            }
            if (first == null || second == null || first.Count != second.Count)
            {
                return false;
            }
            var areEqual = true;
            for (var i = 0; i < first.Count; i++)
            {
                areEqual &= (first[i] == second[i]);
            }
            return areEqual;
        }

        private static void WriteNetworkByteOrder(IList<byte> buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value);
        }

        private static uint ReadNetworkByteOrder(IReadOnlyList<byte> buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                   | ((uint)(buffer[offset + 1]) << 16)
                   | ((uint)(buffer[offset + 2]) << 8)
                   | buffer[offset + 3];
        }
    }
}