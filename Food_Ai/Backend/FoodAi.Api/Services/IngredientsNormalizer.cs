using System.Text.RegularExpressions;

namespace FoodAi.Api.Services;

public interface IIngredientsNormalizer
{
    IReadOnlyList<string> Normalize(IEnumerable<string>? ingredients);
}

public sealed class IngredientsNormalizer : IIngredientsNormalizer
{
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    public IReadOnlyList<string> Normalize(IEnumerable<string>? ingredients)
    {
        if (ingredients is null)
        {
            return Array.Empty<string>();
        }

        var output = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var ingredient in ingredients)
        {
            if (string.IsNullOrWhiteSpace(ingredient))
            {
                continue;
            }

            var parts = ingredient.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var part in parts)
            {
                var normalized = NormalizeValue(part);
                if (string.IsNullOrWhiteSpace(normalized))
                {
                    continue;
                }

                if (seen.Add(normalized))
                {
                    output.Add(normalized);
                }
            }
        }

        return output;
    }

    private static string NormalizeValue(string value)
    {
        var trimmed = value.Trim().ToLowerInvariant();
        return WhitespaceRegex.Replace(trimmed, " ");
    }
}
