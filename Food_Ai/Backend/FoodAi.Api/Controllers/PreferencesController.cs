using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodAi.Api.Contracts;
using FoodAi.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodAi.Api.Controllers;

[ApiController]
[Route("api/preferences")]
[Authorize]
public sealed class PreferencesController : ControllerBase
{
    private readonly IPreferencesService _preferencesService;

    public PreferencesController(IPreferencesService preferencesService)
    {
        _preferencesService = preferencesService;
    }

    [HttpPut]
    public async Task<IActionResult> Save([FromBody] PreferencesRequest request, CancellationToken cancellationToken)
    {
        var userId = TryGetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "User context missing." });
        }

        var saved = await _preferencesService.SaveAsync(userId.Value, request, cancellationToken);
        var response = new PreferencesResponse(saved.Dietary, saved.Allergies, saved.CuisinePreferences);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var userId = TryGetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "User context missing." });
        }

        var preferences = await _preferencesService.GetAsync(userId.Value, cancellationToken);
        if (preferences is null)
        {
            return Ok(new PreferencesResponse(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>()));
        }

        return Ok(new PreferencesResponse(preferences.Dietary, preferences.Allergies, preferences.CuisinePreferences));
    }

    private Guid? TryGetUserId()
    {
        var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim is null)
        {
            return null;
        }

        return Guid.TryParse(claim.Value, out var parsed) ? parsed : null;
    }
}
