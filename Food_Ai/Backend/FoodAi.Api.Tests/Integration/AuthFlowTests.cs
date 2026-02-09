using System.Net;
using System.Net.Http.Json;
using FoodAi.Api.Contracts;
using FluentAssertions;

namespace FoodAi.Api.Tests.Integration;

public sealed class AuthFlowTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthFlowTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_then_login_returns_token()
    {
        var register = new RegisterRequest("Test User", "testuser@example.com", "Password1!", "Password1!");
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", register);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var login = new LoginRequest("testuser@example.com", "Password1!");
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", login);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        payload.Should().NotBeNull();
        payload!.Token.Should().NotBeNullOrWhiteSpace();
        payload.ExpiresInSeconds.Should().BeGreaterThan(0);
    }
}
