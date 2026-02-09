using System.Text.Json;
using FoodAi.Api.Contracts;

namespace FoodAi.Api.Services;

public interface IRecipeSearchService
{
    Task<(bool Success, RecipeSearchResponse? Response, string? Error)> SearchAsync(
        Guid userId,
        RecipeSearchRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class RecipeSearchService : IRecipeSearchService
{
    private readonly IIngredientsNormalizer _ingredientsNormalizer;
    private readonly IRecipeClient _recipeClient;
    private readonly IRecipeCacheService _cacheService;

    public RecipeSearchService(
        IIngredientsNormalizer ingredientsNormalizer,
        IRecipeClient recipeClient,
        IRecipeCacheService cacheService)
    {
        _ingredientsNormalizer = ingredientsNormalizer;
        _recipeClient = recipeClient;
        _cacheService = cacheService;
    }

    public async Task<(bool Success, RecipeSearchResponse? Response, string? Error)> SearchAsync(
        Guid userId,
        RecipeSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var ingredients = _ingredientsNormalizer.Normalize(request.Ingredients);
        if (ingredients.Count == 0)
        {
            return (false, null, "Ingredients are required.");
        }

        var mappedPreferences = PreferencesMapper.Map(userId, request.Preferences, DateTime.UtcNow);
        var cacheKey = BuildCacheKey(ingredients, mappedPreferences);

        var cached = await _cacheService.GetAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cached))
        {
            var cachedResponse = JsonSerializer.Deserialize<RecipeSearchResponse>(cached);
            if (cachedResponse is not null)
            {
                return (true, cachedResponse, null);
            }
        }

        try
        {
            var result = await _recipeClient.SearchRecipesAsync(
                ingredients,
                mappedPreferences.Dietary,
                mappedPreferences.Allergies,
                mappedPreferences.CuisinePreferences,
                cancellationToken);

            var response = new RecipeSearchResponse(result.Recipes.Select(MapRecipe).ToList());
            var responseJson = JsonSerializer.Serialize(response);
            await _cacheService.SetAsync(cacheKey, responseJson, cancellationToken);

            return (true, response, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Recipe provider error: {ex.Message}");
        }
    }

    private static Recipe MapRecipe(RecipeSummary summary)
    {
        return new Recipe(
            summary.Id,
            summary.Name,
            summary.Ingredients.ToList(),
            summary.Steps.ToList(),
            summary.PrepTime,
            summary.Difficulty);
    }

    private static string BuildCacheKey(IReadOnlyList<string> ingredients, FoodAi.Domain.Models.Preferences preferences)
    {
        var ingredientsKey = string.Join(",", ingredients.OrderBy(x => x, StringComparer.OrdinalIgnoreCase));
        var dietaryKey = string.Join(",", preferences.Dietary.OrderBy(x => x, StringComparer.OrdinalIgnoreCase));
        var allergiesKey = string.Join(",", preferences.Allergies.OrderBy(x => x, StringComparer.OrdinalIgnoreCase));
        var cuisineKey = string.Join(",", preferences.CuisinePreferences.OrderBy(x => x, StringComparer.OrdinalIgnoreCase));

        return $"{ingredientsKey}|{dietaryKey}|{allergiesKey}|{cuisineKey}";
    }
}
