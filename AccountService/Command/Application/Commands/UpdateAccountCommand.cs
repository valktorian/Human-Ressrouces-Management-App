namespace AccountService.Command.Application.Commands;

public record UpdateAccountCommand(
    Guid AccountId,
    string FirstName,
    string LastName,
    string Email);
