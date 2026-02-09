using System.Collections.Concurrent;
using FoodAi.Api.Services;

namespace FoodAi.Api.Tests.Integration;

public sealed class TestEmailSender : IEmailSender
{
    private readonly ConcurrentDictionary<string, string> _tokens = new();

    public Task SendPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default)
    {
        _tokens[email] = resetToken;
        return Task.CompletedTask;
    }

    public string? GetLastToken(string email)
    {
        return _tokens.TryGetValue(email, out var token) ? token : null;
    }
}
