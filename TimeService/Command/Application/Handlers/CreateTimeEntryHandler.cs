using Infrastructure.Api.Authentication;
using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;
using TimeService.Command.Domain.Events;

namespace TimeService.Command.Application.Handlers;

public class CreateTimeEntryHandler : ICommandHandler<CreateTimeEntryCommand, CommandAcceptedResponse>
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public CreateTimeEntryHandler(IKafkaProducer kafkaProducer, ICurrentUserAccessor currentUserAccessor)
    {
        _kafkaProducer = kafkaProducer;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task<CommandAcceptedResponse> HandleAsync(CreateTimeEntryCommand command, CancellationToken cancellationToken = default)
    {
        var timeEntryId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var hours = Math.Round((decimal)(command.EndTime - command.StartTime).TotalHours, 2, MidpointRounding.AwayFromZero);
        var accountId = _currentUserAccessor.GetRequiredAccountId();

        var evt = new TimeEntryCreatedEvent
        {
            TimeEntryId = timeEntryId,
            AccountId = accountId,
            EmployeeId = command.EmployeeId,
            WorkDate = command.WorkDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            StartTime = command.StartTime.ToString("HH:mm"),
            EndTime = command.EndTime.ToString("HH:mm"),
            Hours = hours,
            ProjectCode = command.ProjectCode,
            TaskCode = command.TaskCode,
            Notes = command.Notes,
            Status = "Draft",
            CreatedAt = now,
            UpdatedAt = now
        };

        await _kafkaProducer.ProduceAsync(evt, evt, "time.events");

        return new CommandAcceptedResponse(timeEntryId, evt.Status, "Time entry create request accepted.", evt);
    }
}
