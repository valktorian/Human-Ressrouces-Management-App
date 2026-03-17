namespace ProfileService.Command.Application.Commands;

public record LinkProfileAccountCommand(Guid ProfileId, Guid AccountId);
