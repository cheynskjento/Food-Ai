using FoodAi.Api.Contracts;
using FoodAi.Api.Data;
using FoodAi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodAi.Api.Services;

public interface IShoppingListService
{
    Task<(bool Success, ShoppingListItemResponse? Item, string? Error)> AddAsync(
        Guid userId,
        ShoppingListAddRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShoppingListItemResponse>> GetAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<(bool Success, ShoppingListItemResponse? Item, string? Error)> ToggleAsync(
        Guid userId,
        Guid itemId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string? Error)> RemoveAsync(
        Guid userId,
        Guid itemId,
        CancellationToken cancellationToken = default);
}

public sealed class ShoppingListService : IShoppingListService
{
    private readonly FoodAiDbContext _dbContext;

    public ShoppingListService(FoodAiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(bool Success, ShoppingListItemResponse? Item, string? Error)> AddAsync(
        Guid userId,
        ShoppingListAddRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Recipe is null || string.IsNullOrWhiteSpace(request.Recipe.Name))
        {
            return (false, null, "Recipe details are required.");
        }

        var recipeJson = ShoppingListMapper.Serialize(request.Recipe, request.MissingIngredients);
        var entity = new ShoppingListItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RecipeData = recipeJson,
            AddedAt = DateTime.UtcNow,
            IsChecked = false
        };

        _dbContext.ShoppingListItems.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = MapResponse(entity);
        return (true, response, null);
    }

    public async Task<IReadOnlyList<ShoppingListItemResponse>> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.ShoppingListItems
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AddedAt)
            .ToListAsync(cancellationToken);

        return items.Select(MapResponse).ToList();
    }

    public async Task<(bool Success, ShoppingListItemResponse? Item, string? Error)> ToggleAsync(
        Guid userId,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ShoppingListItems.FirstOrDefaultAsync(
            x => x.UserId == userId && x.Id == itemId,
            cancellationToken);

        if (entity is null)
        {
            return (false, null, "Item not found.");
        }

        entity.IsChecked = !entity.IsChecked;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (true, MapResponse(entity), null);
    }

    public async Task<(bool Success, string? Error)> RemoveAsync(
        Guid userId,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ShoppingListItems.FirstOrDefaultAsync(
            x => x.UserId == userId && x.Id == itemId,
            cancellationToken);

        if (entity is null)
        {
            return (false, "Item not found.");
        }

        _dbContext.ShoppingListItems.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    private static ShoppingListItemResponse MapResponse(ShoppingListItem item)
    {
        var payload = ShoppingListMapper.Deserialize(item.RecipeData);
        var recipe = payload?.Recipe ?? new Recipe(string.Empty, "Recipe", Array.Empty<string>(), Array.Empty<string>(), 0, string.Empty);
        var missing = payload?.MissingIngredients ?? Array.Empty<string>();

        return new ShoppingListItemResponse(item.Id, recipe, missing, item.AddedAt, item.IsChecked);
    }
}
