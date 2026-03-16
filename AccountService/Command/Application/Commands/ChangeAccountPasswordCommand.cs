namespace AccountService.Command.Application.Commands;

public record ChangeAccountPasswordCommand(
    Guid AccountId,
    string Password);
