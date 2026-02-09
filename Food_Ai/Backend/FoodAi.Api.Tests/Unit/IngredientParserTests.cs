using FoodAi.Api.Services;
using FluentAssertions;

namespace FoodAi.Api.Tests.Unit;

public sealed class IngredientParserTests
{
    [Fact]
    public void Normalize_removes_duplicates_and_splits_commas()
    {
        var normalizer = new IngredientsNormalizer();
        var input = new[] { " Tomato ", "tomato", "Basil, Garlic", "  ", ",", "Onion" };

        var normalized = normalizer.Normalize(input);

        normalized.Should().Equal("tomato", "basil", "garlic", "onion");
    }

    [Fact]
    public void Normalize_returns_empty_for_null()
    {
        var normalizer = new IngredientsNormalizer();

        var normalized = normalizer.Normalize(null);

        normalized.Should().BeEmpty();
    }
}
