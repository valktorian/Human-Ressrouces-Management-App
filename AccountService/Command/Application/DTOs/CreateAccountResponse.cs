namespace AccountService.Command.Application.DTOs;

public record CreateAccountResponse(
    Guid AccountId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool IsActive,
    DateTime CreatedAt);
