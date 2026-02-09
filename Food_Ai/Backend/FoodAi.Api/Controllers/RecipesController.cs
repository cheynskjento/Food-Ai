using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodAi.Api.Contracts;
using FoodAi.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FoodAi.Api.Controllers;

[ApiController]
[Route("api/recipes")]
[Authorize]
public sealed class RecipesController : ControllerBase
{
    private readonly IRecipeSearchService _recipeSearchService;

    public RecipesController(IRecipeSearchService recipeSearchService)
    {
        _recipeSearchService = recipeSearchService;
    }

    [HttpPost("search")]
    [EnableRateLimiting("recipe-search")]
    public async Task<IActionResult> Search([FromBody] RecipeSearchRequest request, CancellationToken cancellationToken)
    {
        var userId = TryGetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "User context missing." });
        }

        var (success, response, error) = await _recipeSearchService.SearchAsync(userId.Value, request, cancellationToken);
        if (!success || response is null)
        {
            if (!string.IsNullOrWhiteSpace(error) && error.StartsWith("Recipe provider error", StringComparison.Ordinal))
            {
                return StatusCode(StatusCodes.Status502BadGateway, new { error });
            }

            return BadRequest(new { error = error ?? "Invalid request." });
        }

        return Ok(response);
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
