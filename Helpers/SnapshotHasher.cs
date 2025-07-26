using System.Security.Cryptography;
using System.Text;

namespace Helpers
{
    public static class SnapshotHasher
    {
        public static string ComputeHash(string detailsJson)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(detailsJson ?? "");
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
