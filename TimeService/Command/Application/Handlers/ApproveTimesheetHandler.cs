using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class ApproveTimesheetHandler : ICommandHandler<ApproveTimesheetCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(ApproveTimesheetCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(command.Id, "Approved", "Timesheet approve request accepted.", command));
}
