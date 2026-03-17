using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class CreateTimesheetHandler : ICommandHandler<CreateTimesheetCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(CreateTimesheetCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(Guid.NewGuid(), "Draft", "Timesheet create request accepted.", command));
}
