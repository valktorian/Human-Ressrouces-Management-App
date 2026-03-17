using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class RejectTimesheetHandler : ICommandHandler<RejectTimesheetCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(RejectTimesheetCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(command.Id, "Rejected", "Timesheet reject request accepted.", command));
}
