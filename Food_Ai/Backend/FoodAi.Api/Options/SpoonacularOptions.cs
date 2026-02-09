namespace FoodAi.Api.Options;

public sealed class SpoonacularOptions
{
    public const string SectionName = "Spoonacular";

    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.spoonacular.com";
}
