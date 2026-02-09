namespace FoodAi.Domain.Models;

public sealed class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Preferences? Preferences { get; set; }
    public List<ShoppingListItem> ShoppingListItems { get; set; } = new();
    public List<PasswordResetToken> PasswordResetTokens { get; set; } = new();
}
