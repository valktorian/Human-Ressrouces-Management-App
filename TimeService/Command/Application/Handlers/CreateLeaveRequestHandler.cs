using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class CreateLeaveRequestHandler : ICommandHandler<CreateLeaveRequestCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(CreateLeaveRequestCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(Guid.NewGuid(), "Draft", "Leave request create request accepted.", command));
}
