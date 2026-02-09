using System.Net.Http.Json;
using System.Text.Json;
using FoodAi.Api.Options;
using Microsoft.Extensions.Options;

namespace FoodAi.Api.Services;

public sealed class SpoonacularClient : IRecipeClient
{
    private readonly HttpClient _httpClient;
    private readonly SpoonacularOptions _options;

    public SpoonacularClient(HttpClient httpClient, IOptions<SpoonacularOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<RecipeSearchResult> SearchRecipesAsync(
        IReadOnlyList<string> ingredients,
        IReadOnlyList<string> dietary,
        IReadOnlyList<string> allergies,
        IReadOnlyList<string> cuisines,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("Spoonacular API key is not configured.");
        }

        var includeIngredients = string.Join(",", ingredients);
        var diet = dietary.Count > 0 ? string.Join(",", dietary) : null;
        var intolerances = allergies.Count > 0 ? string.Join(",", allergies) : null;
        var cuisine = cuisines.Count > 0 ? string.Join(",", cuisines) : null;

        var query = new Dictionary<string, string?>
        {
            ["apiKey"] = _options.ApiKey,
            ["includeIngredients"] = includeIngredients,
            ["diet"] = diet,
            ["intolerances"] = intolerances,
            ["cuisine"] = cuisine,
            ["addRecipeInformation"] = "true",
            ["number"] = "5"
        };

        var queryString = string.Join("&", query.Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));

        var url = $"{_options.BaseUrl.TrimEnd('/')}/recipes/complexSearch?{queryString}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var root = await JsonSerializer.DeserializeAsync<SpoonacularSearchResponse>(contentStream, cancellationToken: cancellationToken);

        var recipes = root?.Results?.Select(ToRecipeSummary).ToList() ?? new List<RecipeSummary>();
        return new RecipeSearchResult(recipes);
    }

    private static RecipeSummary ToRecipeSummary(SpoonacularRecipe recipe)
    {
        var ingredients = recipe.ExtendedIngredients?
            .Select(i => i.Original ?? string.Empty)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList() ?? new List<string>();
        var steps = recipe.AnalyzedInstructions?.SelectMany(i => i.Steps ?? new List<SpoonacularStep>())
            .Select(s => s.Step)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList() ?? new List<string>();

        return new RecipeSummary(
            recipe.Id.ToString(),
            recipe.Title ?? string.Empty,
            ingredients,
            steps,
            recipe.ReadyInMinutes ?? 0,
            recipe.Difficulty ?? "");
    }

    private sealed class SpoonacularSearchResponse
    {
        public List<SpoonacularRecipe>? Results { get; set; }
    }

    private sealed class SpoonacularRecipe
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? ReadyInMinutes { get; set; }
        public string? Difficulty { get; set; }
        public List<SpoonacularIngredient>? ExtendedIngredients { get; set; }
        public List<SpoonacularInstruction>? AnalyzedInstructions { get; set; }
    }

    private sealed class SpoonacularIngredient
    {
        public string? Original { get; set; }
    }

    private sealed class SpoonacularInstruction
    {
        public List<SpoonacularStep>? Steps { get; set; }
    }

    private sealed class SpoonacularStep
    {
        public string Step { get; set; } = string.Empty;
    }
}
