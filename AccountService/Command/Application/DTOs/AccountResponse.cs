namespace AccountService.Command.Application.DTOs;

public record AccountResponse(
    Guid AccountId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
