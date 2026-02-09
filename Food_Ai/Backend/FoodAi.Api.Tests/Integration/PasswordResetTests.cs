using System.Net;
using System.Net.Http.Json;
using FoodAi.Api.Contracts;
using FoodAi.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace FoodAi.Api.Tests.Integration;

public sealed class PasswordResetTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PasswordResetTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Request_and_confirm_password_reset_allows_login()
    {
        var email = "resetuser@example.com";
        var register = new RegisterRequest("Reset User", email, "OldPassword1!", "OldPassword1!");
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", register);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var requestResponse = await _client.PostAsJsonAsync("/api/auth/password-reset/request", new PasswordResetRequest(email));
        requestResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var sender = _factory.Services.GetRequiredService<IEmailSender>() as TestEmailSender;
        sender.Should().NotBeNull();

        var token = sender!.GetLastToken(email);
        token.Should().NotBeNullOrWhiteSpace();

        var confirmResponse = await _client.PostAsJsonAsync("/api/auth/password-reset/confirm", new PasswordResetConfirm(token!, "NewPassword1!"));
        confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "NewPassword1!"));
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
