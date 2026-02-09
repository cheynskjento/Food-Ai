namespace FoodAi.Domain.Models;

public sealed class RecipeCache
{
    public Guid Id { get; set; }
    public string CacheKey { get; set; } = string.Empty;
    public string ResponseJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
