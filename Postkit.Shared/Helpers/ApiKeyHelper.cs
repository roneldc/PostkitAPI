using System.Security.Cryptography;
using System.Text;

namespace Postkit.Shared.Helpers
{
    public static class ApiKeyHelper
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
    }

}
