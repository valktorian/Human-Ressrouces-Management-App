using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class SubmitTimesheetHandler : ICommandHandler<SubmitTimesheetCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(SubmitTimesheetCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(command.Id, "Submitted", "Timesheet submit request accepted."));
}
