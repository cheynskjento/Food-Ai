using System.Security.Cryptography;
using System.Text;

namespace FoodAi.Api.Services;

public static class PasswordResetTokenHasher
{
    public static string HashToken(string token)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public static bool Verify(string token, string hash)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        return string.Equals(HashToken(token), hash, StringComparison.Ordinal);
    }
}
