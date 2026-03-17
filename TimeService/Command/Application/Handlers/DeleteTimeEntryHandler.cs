using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class DeleteTimeEntryHandler : ICommandHandler<DeleteTimeEntryCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(DeleteTimeEntryCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(command.Id, "Deleted", "Time entry delete request accepted."));
}
