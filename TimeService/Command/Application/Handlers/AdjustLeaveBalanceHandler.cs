using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;
using TimeService.Command.Domain.Events;

namespace TimeService.Command.Application.Handlers;

public class AdjustLeaveBalanceHandler : ICommandHandler<AdjustLeaveBalanceCommand, CommandAcceptedResponse>
{
    private readonly IKafkaProducer _kafkaProducer;

    public AdjustLeaveBalanceHandler(IKafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }

    public async Task<CommandAcceptedResponse> HandleAsync(AdjustLeaveBalanceCommand command, CancellationToken cancellationToken = default)
    {
        var evt = new LeaveBalanceAdjustedEvent
        {
            LeaveBalanceId = Guid.NewGuid(),
            AccountId = command.EmployeeId,
            EmployeeId = command.EmployeeId,
            LeaveType = command.LeaveType,
            Available = command.Delta,
            Used = 0m,
            Pending = 0m,
            Delta = command.Delta,
            Reason = command.Reason,
            UpdatedAt = DateTime.UtcNow
        };

        await _kafkaProducer.ProduceAsync(evt, evt, "time.events");
        return new CommandAcceptedResponse(command.EmployeeId, "Adjusted", "Leave balance adjustment accepted.", evt);
    }
}
