using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class CancelLeaveRequestHandler : ICommandHandler<CancelLeaveRequestCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(CancelLeaveRequestCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(command.Id, "Cancelled", "Leave request cancel accepted.", command));
}
