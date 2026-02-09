using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodAi.Api.Contracts;
using FoodAi.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodAi.Api.Controllers;

[ApiController]
[Route("api/shoppinglist")]
[Authorize]
public sealed class ShoppingListController : ControllerBase
{
    private readonly IShoppingListService _shoppingListService;

    public ShoppingListController(IShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] ShoppingListAddRequest request, CancellationToken cancellationToken)
    {
        var userId = TryGetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "User context missing." });
        }

        var (success, item, error) = await _shoppingListService.AddAsync(userId.Value, request, cancellationToken);
        if (!success || item is null)
        {
            return BadRequest(new { error = error ?? "Invalid request." });
        }

        return Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var userId = TryGetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "User context missing." });
        }

        var items = await _shoppingListService.GetAsync(userId.Value, cancellationToken);
        return Ok(items);
    }

    [HttpPost("{id:guid}/toggle")]
    public async Task<IActionResult> Toggle([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = TryGetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "User context missing." });
        }

        var (success, item, error) = await _shoppingListService.ToggleAsync(userId.Value, id, cancellationToken);
        if (!success || item is null)
        {
            return NotFound(new { error = error ?? "Item not found." });
        }

        return Ok(item);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = TryGetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "User context missing." });
        }

        var (success, error) = await _shoppingListService.RemoveAsync(userId.Value, id, cancellationToken);
        if (!success)
        {
            return NotFound(new { error = error ?? "Item not found." });
        }

        return NoContent();
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
