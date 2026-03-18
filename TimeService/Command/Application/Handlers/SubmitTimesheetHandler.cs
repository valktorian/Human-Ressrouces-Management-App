using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;
using TimeService.Command.Domain.Events;

namespace TimeService.Command.Application.Handlers;

public class SubmitTimesheetHandler : ICommandHandler<SubmitTimesheetCommand, CommandAcceptedResponse>
{
    private readonly IKafkaProducer _kafkaProducer;

    public SubmitTimesheetHandler(IKafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }

    public async Task<CommandAcceptedResponse> HandleAsync(SubmitTimesheetCommand command, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var evt = new TimesheetStatusChangedEvent
        {
            TimesheetId = command.Id,
            Status = "Submitted",
            SubmittedAt = now,
            UpdatedAt = now
        };

        await _kafkaProducer.ProduceAsync(evt, evt, "time.events");
        return new CommandAcceptedResponse(command.Id, evt.Status, "Timesheet submit request accepted.", evt);
    }
}
