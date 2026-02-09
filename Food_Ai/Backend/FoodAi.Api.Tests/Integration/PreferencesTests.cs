using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FoodAi.Api.Contracts;
using FoodAi.Api.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoodAi.Api.Tests.Integration;

public sealed class PreferencesTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PreferencesTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Save_preferences_persists_values()
    {
        var email = $"prefs-{Guid.NewGuid():N}@example.com";
        var token = await RegisterAndLoginAsync(email);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new PreferencesRequest(
            new[] { "Vegan", "vegan " },
            new[] { "Peanut" },
            new[] { " Italian " });

        var response = await _client.PutAsJsonAsync("/api/preferences", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FoodAiDbContext>();
        var user = await dbContext.Users.FirstAsync(u => u.NormalizedEmail == email.ToLowerInvariant());
        var preferences = await dbContext.Preferences.FirstOrDefaultAsync(p => p.UserId == user.Id);

        preferences.Should().NotBeNull();
        preferences!.Dietary.Should().Equal("vegan");
        preferences.Allergies.Should().Equal("peanut");
        preferences.CuisinePreferences.Should().Equal("italian");
    }

    [Fact]
    public async Task Get_preferences_returns_saved_values()
    {
        var email = $"prefsget-{Guid.NewGuid():N}@example.com";
        var token = await RegisterAndLoginAsync(email);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new PreferencesRequest(
            new[] { "Vegan" },
            new[] { "Shellfish" },
            new[] { "Asian" });

        var saveResponse = await _client.PutAsJsonAsync("/api/preferences", request);
        saveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await _client.GetAsync("/api/preferences");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await getResponse.Content.ReadFromJsonAsync<PreferencesResponse>();
        payload.Should().NotBeNull();
        payload!.Dietary.Should().Equal("vegan");
        payload.Allergies.Should().Equal("shellfish");
        payload.CuisinePreferences.Should().Equal("asian");
    }

    private async Task<string> RegisterAndLoginAsync(string email)
    {
        var register = new RegisterRequest("Preference User", email, "Password1!", "Password1!");
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", register);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var login = new LoginRequest(email, "Password1!");
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", login);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        payload.Should().NotBeNull();
        payload!.Token.Should().NotBeNullOrWhiteSpace();
        return payload.Token;
    }
}
