using System.Text.RegularExpressions;

namespace FoodAi.Api.Validation;

public static class Validators
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public static bool IsValidEmail(string? email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
    }

    public static bool IsValidPassword(string? password)
    {
        return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
    }

    public static bool HasValue(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
