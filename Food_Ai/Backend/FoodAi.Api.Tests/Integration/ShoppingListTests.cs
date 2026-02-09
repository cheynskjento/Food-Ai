using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FoodAi.Api.Contracts;
using FluentAssertions;

namespace FoodAi.Api.Tests.Integration;

public sealed class ShoppingListTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ShoppingListTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Shopping_list_crud_flow()
    {
        var email = $"shop-{Guid.NewGuid():N}@example.com";
        var token = await RegisterAndLoginAsync(email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var recipe = new Recipe(
            "recipe-1",
            "Shopping Recipe",
            new[] { "tomato", "basil" },
            new[] { "mix", "cook" },
            20,
            "easy");

        var addRequest = new ShoppingListAddRequest(recipe, new[] { "basil" });
        var addResponse = await _client.PostAsJsonAsync("/api/shoppinglist/add", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var added = await addResponse.Content.ReadFromJsonAsync<ShoppingListItemResponse>();
        added.Should().NotBeNull();
        added!.Recipe.Name.Should().Be("Shopping Recipe");
        added.IsChecked.Should().BeFalse();

        var listResponse = await _client.GetAsync("/api/shoppinglist");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await listResponse.Content.ReadFromJsonAsync<List<ShoppingListItemResponse>>();
        list.Should().NotBeNull();
        list!.Should().ContainSingle();

        var toggleResponse = await _client.PostAsync($"/api/shoppinglist/{added.Id}/toggle", null);
        toggleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var toggled = await toggleResponse.Content.ReadFromJsonAsync<ShoppingListItemResponse>();
        toggled.Should().NotBeNull();
        toggled!.IsChecked.Should().BeTrue();

        var deleteResponse = await _client.DeleteAsync($"/api/shoppinglist/{added.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var listAfterDelete = await _client.GetAsync("/api/shoppinglist");
        var after = await listAfterDelete.Content.ReadFromJsonAsync<List<ShoppingListItemResponse>>();
        after.Should().NotBeNull();
        after!.Should().BeEmpty();
    }

    private async Task<string> RegisterAndLoginAsync(string email)
    {
        var register = new RegisterRequest("Shopper", email, "Password1!", "Password1!");
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", register);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var login = new LoginRequest(email, "Password1!");
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", login);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        payload.Should().NotBeNull();
        return payload!.Token;
    }
}
