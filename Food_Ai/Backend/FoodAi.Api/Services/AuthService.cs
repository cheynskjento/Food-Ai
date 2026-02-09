using FoodAi.Api.Contracts;
using FoodAi.Api.Data;
using FoodAi.Api.Validation;
using FoodAi.Domain.Models;
using FoodAi.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodAi.Api.Services;

public interface IAuthService
{
    Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<(bool Success, LoginResponse? Response, string? Error)> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public sealed class AuthService : IAuthService
{
    private readonly FoodAiDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(FoodAiDbContext dbContext, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (!Validators.HasValue(request.Name))
        {
            return (false, "Name is required.");
        }

        if (!Validators.IsValidEmail(request.Email))
        {
            return (false, "Email is invalid.");
        }

        if (!Validators.IsValidPassword(request.Password))
        {
            return (false, "Password is too short.");
        }

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return (false, "Passwords do not match.");
        }

        var normalizedEmail = Validators.NormalizeEmail(request.Email);
        var exists = await _dbContext.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        if (exists)
        {
            return (false, "Email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (true, null);
    }

    public async Task<(bool Success, LoginResponse? Response, string? Error)> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (!Validators.IsValidEmail(request.Email) || !Validators.IsValidPassword(request.Password))
        {
            return (false, null, "Invalid credentials.");
        }

        var normalizedEmail = Validators.NormalizeEmail(request.Email);
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        if (user is null)
        {
            return (false, null, "Invalid credentials.");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return (false, null, "Invalid credentials.");
        }

        var (token, expiresInSeconds) = _jwtTokenService.CreateToken(user.Id, user.Email);
        return (true, new LoginResponse(token, expiresInSeconds), null);
    }
}
