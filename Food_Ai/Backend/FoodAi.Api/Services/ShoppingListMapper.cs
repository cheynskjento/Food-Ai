using System.Text.Json;
using FoodAi.Api.Contracts;

namespace FoodAi.Api.Services;

public static class ShoppingListMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static string Serialize(Recipe recipe, IReadOnlyList<string>? missingIngredients)
    {
        var payload = new ShoppingListPayload(
            recipe,
            missingIngredients?.ToList() ?? new List<string>());
        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    public static ShoppingListPayload? Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<ShoppingListPayload>(json, JsonOptions);
    }
}

public sealed record ShoppingListPayload(
    Recipe Recipe,
    IReadOnlyList<string> MissingIngredients);
