using FoodAi.Api.Contracts;
using FoodAi.Api.Services;
using FluentAssertions;

namespace FoodAi.Api.Tests.Unit;

public sealed class PreferencesMappingTests
{
    [Fact]
    public void Map_normalizes_and_deduplicates_lists()
    {
        var userId = Guid.NewGuid();
        var updatedAt = new DateTime(2026, 2, 9, 12, 0, 0, DateTimeKind.Utc);
        var request = new PreferencesRequest(
            new[] { " Vegan ", "vegan", "  " },
            new[] { "Peanut", "peanut", " " },
            new[] { " Italian ", "italian", "Mediterranean" });

        var mapped = PreferencesMapper.Map(userId, request, updatedAt);

        mapped.UserId.Should().Be(userId);
        mapped.UpdatedAt.Should().Be(updatedAt);
        mapped.Dietary.Should().Equal("vegan");
        mapped.Allergies.Should().Equal("peanut");
        mapped.CuisinePreferences.Should().Equal("italian", "mediterranean");
    }
}
