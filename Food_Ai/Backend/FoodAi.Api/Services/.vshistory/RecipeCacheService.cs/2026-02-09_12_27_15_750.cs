using FoodAi.Api.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace FoodAi.Api.Services;

public interface IRecipeCacheService
{
    Task<string?> GetAsync(string cacheKey, CancellationToken cancellationToken = default);
    Task SetAsync(string cacheKey, string responseJson, CancellationToken cancellationToken = default);
}

public sealed class RecipeCacheService : IRecipeCacheService
{
    private readonly IMemoryCache _cache;
    private readonly CacheOptions _options;

    public RecipeCacheService(IMemoryCache cache, IOptions<CacheOptions> options)
    {
        _cache = cache;
        _options = options.Value;
    }

    public Task<string?> GetAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(cacheKey, out string? value);
        return Task.FromResult(value);
    }

    public Task SetAsync(string cacheKey, string responseJson, CancellationToken cancellationToken = default)
    {
        var ttl = TimeSpan.FromHours(_options.RecipeHours);
        _cache.Set(cacheKey, responseJson, ttl);
        return Task.CompletedTask;
    }
}
