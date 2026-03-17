using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class UpdateTimeEntryHandler : ICommandHandler<UpdateTimeEntryCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(UpdateTimeEntryCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(command.Id, "Draft", "Time entry update request accepted.", command));
}
