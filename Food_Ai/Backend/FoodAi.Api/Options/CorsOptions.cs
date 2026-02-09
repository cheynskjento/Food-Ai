namespace FoodAi.Api.Options;

public sealed class CorsOptions
{
    public const string SectionName = "Cors";

    public string Origin { get; set; } = "http://localhost:8080";
}
