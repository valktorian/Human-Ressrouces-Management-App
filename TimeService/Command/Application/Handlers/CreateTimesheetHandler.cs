using Infrastructure.Api.Authentication;
using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;
using TimeService.Command.Domain.Events;

namespace TimeService.Command.Application.Handlers;

public class CreateTimesheetHandler : ICommandHandler<CreateTimesheetCommand, CommandAcceptedResponse>
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public CreateTimesheetHandler(IKafkaProducer kafkaProducer, ICurrentUserAccessor currentUserAccessor)
    {
        _kafkaProducer = kafkaProducer;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task<CommandAcceptedResponse> HandleAsync(CreateTimesheetCommand command, CancellationToken cancellationToken = default)
    {
        var timesheetId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var accountId = _currentUserAccessor.GetRequiredAccountId();

        var evt = new TimesheetCreatedEvent
        {
            TimesheetId = timesheetId,
            AccountId = accountId,
            EmployeeId = command.EmployeeId,
            PeriodStart = command.PeriodStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            PeriodEnd = command.PeriodEnd.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            TotalHours = 0m,
            Status = "Draft",
            CreatedAt = now,
            UpdatedAt = now
        };

        await _kafkaProducer.ProduceAsync(evt, evt, "time.events");

        return new CommandAcceptedResponse(timesheetId, evt.Status, "Timesheet create request accepted.", evt);
    }
}
