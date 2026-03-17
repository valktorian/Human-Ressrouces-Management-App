using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class SubmitLeaveRequestHandler : ICommandHandler<SubmitLeaveRequestCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(SubmitLeaveRequestCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(command.Id, "Submitted", "Leave request submit accepted."));
}
