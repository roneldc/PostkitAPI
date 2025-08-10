using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace Postkit.Shared.Helpers
{
    public static class SecurityHelper
    {
        public static string HashApiKey(string apiKey)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(apiKey);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyApiKey(string inputApiKey, string storedHash)
        {
            var inputHash = HashApiKey(inputApiKey);
            return inputHash == storedHash;
        }

        public static string GenerateApiKey()
        {
            var keyBytes = RandomNumberGenerator.GetBytes(32);
            return WebEncoders.Base64UrlEncode(keyBytes);
        }

        public static string GenerateEmailToken()
        {
            byte[] randomBytes = new byte[32];
            RandomNumberGenerator.Fill(randomBytes);
            return WebEncoders.Base64UrlEncode(randomBytes);
        }

        public static string GenerateSecurePassword(int length = 12)
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lower = "abcdefghijkmnopqrstuvwxyz";
            const string digit = "23456789";
            const string special = "!@$?_-";
            const string all = upper + lower + digit + special;

            var random = new Random();
            var chars = new List<char>
            {
                upper[random.Next(upper.Length)],
                lower[random.Next(lower.Length)],
                digit[random.Next(digit.Length)],
                special[random.Next(special.Length)]
            };

            for (int i = chars.Count; i < length; i++)
            {
                chars.Add(all[random.Next(all.Length)]);
            }

            return new string(chars.OrderBy(_ => random.Next()).ToArray());
        }

        public static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

}
