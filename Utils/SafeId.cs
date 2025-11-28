using System;
using System.Security.Cryptography;
using System.Text;

namespace NexScore.Utils
{
    public static class SafeId
    {
        // Ambiguity-safe alphabet (no 0/O or 1/l/I)
        private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";

        public static string NewId(int length = 10)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
            var sb = new StringBuilder(length);
            var bytes = new byte[length];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            for (int i = 0; i < length; i++)
            {
                // Map 0-255 to alphabet length
                int idx = bytes[i] % Alphabet.Length;
                sb.Append(Alphabet[idx]);
            }
            return sb.ToString();
        }
    }
}