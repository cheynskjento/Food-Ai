using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FoodAi.Api.Contracts;
using FoodAi.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FoodAi.Api.Tests.Integration;

public sealed class RecipeSearchTests
{
    [Fact]
    public async Task Search_uses_cache_for_repeat_requests()
    {
        var fakeClient = new FakeRecipeClient();
        await using var factory = new TestWebApplicationFactory()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll(typeof(IRecipeClient));
                    services.AddSingleton<IRecipeClient>(fakeClient);
                });
            });

        var client = factory.CreateClient();
        var email = $"recipes-{Guid.NewGuid():N}@example.com";
        var token = await RegisterAndLoginAsync(client, email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new RecipeSearchRequest(
            new[] { "Tomato", "Basil" },
            new PreferencesRequest(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>()));

        var first = await client.PostAsJsonAsync("/api/recipes/search", request);
        first.StatusCode.Should().Be(HttpStatusCode.OK);

        var second = await client.PostAsJsonAsync("/api/recipes/search", request);
        second.StatusCode.Should().Be(HttpStatusCode.OK);

        fakeClient.CallCount.Should().Be(1);
    }

    private static async Task<string> RegisterAndLoginAsync(HttpClient client, string email)
    {
        var register = new RegisterRequest("Recipe User", email, "Password1!", "Password1!");
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", register);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var login = new LoginRequest(email, "Password1!");
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", login);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        payload.Should().NotBeNull();
        return payload!.Token;
    }

    private sealed class FakeRecipeClient : IRecipeClient
    {
        public int CallCount { get; private set; }

        public Task<RecipeSearchResult> SearchRecipesAsync(
            IReadOnlyList<string> ingredients,
            IReadOnlyList<string> dietary,
            IReadOnlyList<string> allergies,
            IReadOnlyList<string> cuisines,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            var recipe = new RecipeSummary(
                "recipe-1",
                "Test Recipe",
                new[] { "ingredient" },
                new[] { "step" },
                10,
                "easy");

            return Task.FromResult(new RecipeSearchResult(new[] { recipe }));
        }
    }
}
