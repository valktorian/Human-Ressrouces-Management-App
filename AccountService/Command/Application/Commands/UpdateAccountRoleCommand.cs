namespace AccountService.Command.Application.Commands;

public record UpdateAccountRoleCommand(
    Guid AccountId,
    string Role);
