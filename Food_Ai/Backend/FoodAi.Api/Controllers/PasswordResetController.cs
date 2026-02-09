using FoodAi.Api.Contracts;
using FoodAi.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodAi.Api.Controllers;

[ApiController]
[Route("api/auth/password-reset")]
public sealed class PasswordResetController : ControllerBase
{
    private readonly IPasswordResetService _passwordResetService;

    public PasswordResetController(IPasswordResetService passwordResetService)
    {
        _passwordResetService = passwordResetService;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestReset([FromBody] PasswordResetRequest request, CancellationToken cancellationToken)
    {
        await _passwordResetService.RequestResetAsync(request.Email, cancellationToken);
        return Ok();
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmReset([FromBody] PasswordResetConfirm request, CancellationToken cancellationToken)
    {
        var (success, error) = await _passwordResetService.ConfirmResetAsync(request.Token, request.NewPassword, cancellationToken);
        if (!success)
        {
            return BadRequest(new { error = error ?? "Invalid or expired token." });
        }

        return Ok();
    }
}
