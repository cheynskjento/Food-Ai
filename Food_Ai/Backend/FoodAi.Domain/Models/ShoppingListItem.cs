namespace FoodAi.Domain.Models;

public sealed class ShoppingListItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RecipeData { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
    public bool IsChecked { get; set; }

    public User? User { get; set; }
}
