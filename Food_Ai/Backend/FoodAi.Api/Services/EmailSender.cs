using Microsoft.Extensions.Logging;

namespace FoodAi.Api.Services;

public interface IEmailSender
{
    Task SendPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default);
}

public sealed class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Password reset requested for {Email}. Token: {Token}", email, resetToken);
        return Task.CompletedTask;
    }
}
