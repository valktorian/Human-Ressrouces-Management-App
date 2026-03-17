using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class RejectLeaveRequestHandler : ICommandHandler<RejectLeaveRequestCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(RejectLeaveRequestCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(command.Id, "Rejected", "Leave request reject accepted.", command));
}
