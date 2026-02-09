namespace FoodAi.Api.Options;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public int RecipeHours { get; set; } = 24;
}
