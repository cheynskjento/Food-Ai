using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FoodAi.Api.Options;
using FoodAi.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FoodAi.Api.Tests.Unit;

public sealed class JwtTokenTests
{
    [Fact]
    public void CreateToken_returns_valid_jwt()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new JwtOptions
        {
            Issuer = "FoodAi",
            Audience = "FoodAi.Users",
            Secret = "super-secret-test-key-1234567890",
            ExpiresMinutes = 60
        });

        var service = new JwtTokenService(options);
        var (token, _) = service.CreateToken(Guid.NewGuid(), "test@example.com");

        var handler = new JwtSecurityTokenHandler();
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = options.Value.Issuer,
            ValidAudience = options.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Secret)),
            ClockSkew = TimeSpan.Zero
        };

        var principal = handler.ValidateToken(token, parameters, out _);
        principal.Identity?.IsAuthenticated.Should().BeTrue();
        principal.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Email && c.Value == "test@example.com");
    }
}
