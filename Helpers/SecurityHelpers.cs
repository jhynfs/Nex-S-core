using System.Security.Cryptography;
using System.Text;

namespace NexScore.Helpers
{
    public static class SecurityHelper
    {
        public static string HashString(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }

        public static string GenerateRecoveryCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return $"NEX-{new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray())}-" +
                   $"{new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray())}";
        }
    }
}
