using FoodAi.Api.Contracts;
using FoodAi.Api.Services;
using FluentAssertions;

namespace FoodAi.Api.Tests.Unit;

public sealed class ShoppingListMappingTests
{
    [Fact]
    public void Serialize_and_deserialize_roundtrip()
    {
        var recipe = new Recipe(
            "recipe-42",
            "Test Recipe",
            new[] { "tomato", "basil" },
            new[] { "step 1", "step 2" },
            15,
            "easy");
        var missing = new[] { "basil" };

        var json = ShoppingListMapper.Serialize(recipe, missing);
        var payload = ShoppingListMapper.Deserialize(json);

        payload.Should().NotBeNull();
        payload!.Recipe.Id.Should().Be(recipe.Id);
        payload.Recipe.Name.Should().Be(recipe.Name);
        payload.Recipe.Ingredients.Should().Equal(recipe.Ingredients);
        payload.Recipe.Steps.Should().Equal(recipe.Steps);
        payload.Recipe.PrepTime.Should().Be(recipe.PrepTime);
        payload.Recipe.Difficulty.Should().Be(recipe.Difficulty);
        payload.MissingIngredients.Should().Equal("basil");
    }
}
