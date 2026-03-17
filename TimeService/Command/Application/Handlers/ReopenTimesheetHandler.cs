using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class ReopenTimesheetHandler : ICommandHandler<ReopenTimesheetCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(ReopenTimesheetCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(command.Id, "Draft", "Timesheet reopen request accepted.", command));
}
