using System.Text.RegularExpressions;
using FoodAi.Api.Contracts;
using FoodAi.Domain.Models;

namespace FoodAi.Api.Services;

public static class PreferencesMapper
{
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    public static Preferences Map(Guid userId, PreferencesRequest request, DateTime updatedAt)
    {
        return new Preferences
        {
            UserId = userId,
            Dietary = NormalizeList(request.Dietary),
            Allergies = NormalizeList(request.Allergies),
            CuisinePreferences = NormalizeList(request.CuisinePreferences),
            UpdatedAt = updatedAt
        };
    }

    private static List<string> NormalizeList(IEnumerable<string>? values)
    {
        if (values is null)
        {
            return new List<string>();
        }

        var normalized = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var cleaned = WhitespaceRegex.Replace(value.Trim().ToLowerInvariant(), " ");
            if (string.IsNullOrWhiteSpace(cleaned))
            {
                continue;
            }

            if (seen.Add(cleaned))
            {
                normalized.Add(cleaned);
            }
        }

        return normalized;
    }
}
