using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Application.Handlers;

public class AdjustLeaveBalanceHandler : ICommandHandler<AdjustLeaveBalanceCommand, CommandAcceptedResponse>
{
    public Task<CommandAcceptedResponse> HandleAsync(AdjustLeaveBalanceCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandAcceptedResponse(command.EmployeeId, "Adjusted", "Leave balance adjustment accepted.", command));
}
