namespace AccountService.Command.Application.Commands;

public record CreateAccountCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role);
