using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class CreateTimeEntryHandler : ICommandHandler<CreateTimeEntryCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(CreateTimeEntryCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(Guid.NewGuid(), "Draft", "Time entry create request accepted.", command));
}
