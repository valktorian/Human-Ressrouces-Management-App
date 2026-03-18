using Infrastructure.Api.Authentication;
using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;
using TimeService.Command.Domain.Events;

namespace TimeService.Command.Application.Handlers;

public class CreateLeaveRequestHandler : ICommandHandler<CreateLeaveRequestCommand, CommandAcceptedResponse>
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public CreateLeaveRequestHandler(IKafkaProducer kafkaProducer, ICurrentUserAccessor currentUserAccessor)
    {
        _kafkaProducer = kafkaProducer;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task<CommandAcceptedResponse> HandleAsync(CreateLeaveRequestCommand command, CancellationToken cancellationToken = default)
    {
        var leaveRequestId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var accountId = _currentUserAccessor.GetRequiredAccountId();

        var evt = new LeaveRequestCreatedEvent
        {
            LeaveRequestId = leaveRequestId,
            AccountId = accountId,
            EmployeeId = command.EmployeeId,
            LeaveType = command.LeaveType,
            StartDate = command.StartDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            EndDate = command.EndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            Status = "Draft",
            Reason = command.Reason,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _kafkaProducer.ProduceAsync(evt, evt, "time.events");

        return new CommandAcceptedResponse(leaveRequestId, evt.Status, "Leave request create request accepted.", evt);
    }
}
