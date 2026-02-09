using FoodAi.Api.Contracts;
using FoodAi.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodAi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var (success, error) = await _authService.RegisterAsync(request, cancellationToken);
        if (!success)
        {
            if (string.Equals(error, "Email already exists.", StringComparison.Ordinal))
            {
                return Conflict(new { error });
            }

            return BadRequest(new { error });
        }

        return Created("/api/auth/register", null);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var (success, response, error) = await _authService.LoginAsync(request, cancellationToken);
        if (!success || response is null)
        {
            return Unauthorized(new { error = error ?? "Invalid credentials." });
        }

        return Ok(response);
    }
}
