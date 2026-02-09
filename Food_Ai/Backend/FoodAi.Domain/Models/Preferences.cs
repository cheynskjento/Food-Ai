namespace FoodAi.Domain.Models;

public sealed class Preferences
{
    public Guid UserId { get; set; }
    public List<string> Dietary { get; set; } = new();
    public List<string> Allergies { get; set; } = new();
    public List<string> CuisinePreferences { get; set; } = new();
    public DateTime UpdatedAt { get; set; }

    public User? User { get; set; }
}
