namespace FoodAi.Api.Contracts;

public sealed record RegisterRequest(string Name, string Email, string Password, string ConfirmPassword);

public sealed record LoginRequest(string Email, string Password);

public sealed record LoginResponse(string Token, int ExpiresInSeconds);

public sealed record PasswordResetRequest(string Email);

public sealed record PasswordResetConfirm(string Token, string NewPassword);
