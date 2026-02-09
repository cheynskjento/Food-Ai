namespace FoodAi.Api.Options;

public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimiting";

    public RateLimitPolicyOptions RecipeSearch { get; set; } = new();
}

public sealed class RateLimitPolicyOptions
{
    public int PermitLimit { get; set; } = 10;
    public int WindowSeconds { get; set; } = 60;
}
