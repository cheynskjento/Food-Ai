namespace FoodAi.Api.Services;

public sealed record RecipeSummary(
    string Id,
    string Name,
    IReadOnlyList<string> Ingredients,
    IReadOnlyList<string> Steps,
    int PrepTime,
    string Difficulty);

public sealed record RecipeSearchResult(IReadOnlyList<RecipeSummary> Recipes);

public interface IRecipeClient
{
    Task<RecipeSearchResult> SearchRecipesAsync(IReadOnlyList<string> ingredients, IReadOnlyList<string> dietary, IReadOnlyList<string> allergies, IReadOnlyList<string> cuisines, CancellationToken cancellationToken = default);
}
