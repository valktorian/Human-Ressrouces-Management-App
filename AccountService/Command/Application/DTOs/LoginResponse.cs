namespace AccountService.Command.Application.DTOs;

public record LoginResponse(
    string AccessToken,
    DateTime ExpiresAt,
    Guid AccountId,
    string Email,
    string Role);
