namespace FoodAi.Api.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "FoodAi";
    public string Audience { get; set; } = "FoodAi.Users";
    public string Secret { get; set; } = string.Empty;
    public int ExpiresMinutes { get; set; } = 60;
}
