namespace FoodAi.Api.Contracts;

public sealed record ShoppingListAddRequest(
    Recipe Recipe,
    IReadOnlyList<string>? MissingIngredients);

public sealed record ShoppingListItemResponse(
    Guid Id,
    Recipe Recipe,
    IReadOnlyList<string> MissingIngredients,
    DateTime AddedAt,
    bool IsChecked);
