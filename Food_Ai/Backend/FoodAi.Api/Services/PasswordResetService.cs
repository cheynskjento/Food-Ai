using System.Security.Cryptography;
using FoodAi.Api.Data;
using FoodAi.Api.Validation;
using FoodAi.Domain.Models;
using FoodAi.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodAi.Api.Services;

public interface IPasswordResetService
{
    Task RequestResetAsync(string email, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> ConfirmResetAsync(string token, string newPassword, CancellationToken cancellationToken = default);
}

public sealed class PasswordResetService : IPasswordResetService
{
    private readonly FoodAiDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailSender _emailSender;

    public PasswordResetService(FoodAiDbContext dbContext, IPasswordHasher passwordHasher, IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _emailSender = emailSender;
    }

    public async Task RequestResetAsync(string email, CancellationToken cancellationToken = default)
    {
        if (!Validators.IsValidEmail(email))
        {
            return;
        }

        var normalizedEmail = Validators.NormalizeEmail(email);
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        if (user is null)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var existingTokens = await _dbContext.PasswordResetTokens
            .Where(t => t.UserId == user.Id && t.UsedAt == null && t.ExpiresAt > now)
            .ToListAsync(cancellationToken);

        foreach (var token in existingTokens)
        {
            token.UsedAt = now;
        }

        var rawToken = GenerateToken();
        var tokenHash = PasswordResetTokenHasher.HashToken(rawToken);

        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAt = now,
            ExpiresAt = now.AddHours(1)
        };

        _dbContext.PasswordResetTokens.Add(resetToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _emailSender.SendPasswordResetAsync(user.Email, rawToken, cancellationToken);
    }

    public async Task<(bool Success, string? Error)> ConfirmResetAsync(string token, string newPassword, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return (false, "Invalid token.");
        }

        if (!Validators.IsValidPassword(newPassword))
        {
            return (false, "Password is too short.");
        }

        var tokenHash = PasswordResetTokenHasher.HashToken(token);
        var now = DateTime.UtcNow;

        var resetToken = await _dbContext.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.UsedAt == null && t.ExpiresAt > now, cancellationToken);

        if (resetToken?.User is null)
        {
            return (false, "Invalid or expired token.");
        }

        resetToken.UsedAt = now;
        resetToken.User.PasswordHash = _passwordHasher.HashPassword(newPassword);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return (true, null);
    }

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}
