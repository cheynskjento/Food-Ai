using FoodAi.Api.Contracts;
using FoodAi.Api.Data;
using FoodAi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodAi.Api.Services;

public interface IPreferencesService
{
    Task<Preferences> SaveAsync(Guid userId, PreferencesRequest request, CancellationToken cancellationToken = default);
    Task<Preferences?> GetAsync(Guid userId, CancellationToken cancellationToken = default);
}

public sealed class PreferencesService : IPreferencesService
{
    private readonly FoodAiDbContext _dbContext;

    public PreferencesService(FoodAiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Preferences> SaveAsync(Guid userId, PreferencesRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Preferences.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        var updatedAt = DateTime.UtcNow;
        var mapped = PreferencesMapper.Map(userId, request, updatedAt);

        if (existing is null)
        {
            _dbContext.Preferences.Add(mapped);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return mapped;
        }

        existing.Dietary = mapped.Dietary;
        existing.Allergies = mapped.Allergies;
        existing.CuisinePreferences = mapped.CuisinePreferences;
        existing.UpdatedAt = updatedAt;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public Task<Preferences?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Preferences.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }
}
