using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class UpdateLeaveRequestHandler : ICommandHandler<UpdateLeaveRequestCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(UpdateLeaveRequestCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(command.Id, "Draft", "Leave request update accepted.", command));
}
