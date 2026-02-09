namespace FoodAi.Api.Contracts;

public sealed record PreferencesRequest(
    IReadOnlyList<string>? Dietary,
    IReadOnlyList<string>? Allergies,
    IReadOnlyList<string>? CuisinePreferences);

public sealed record PreferencesResponse(
    IReadOnlyList<string> Dietary,
    IReadOnlyList<string> Allergies,
    IReadOnlyList<string> CuisinePreferences);

public sealed record RecipeSearchRequest(
    IReadOnlyList<string> Ingredients,
    PreferencesRequest Preferences);

public sealed record Recipe(
    string Id,
    string Name,
    IReadOnlyList<string> Ingredients,
    IReadOnlyList<string> Steps,
    int PrepTime,
    string Difficulty);

public sealed record RecipeSearchResponse(IReadOnlyList<Recipe> Recipes);
