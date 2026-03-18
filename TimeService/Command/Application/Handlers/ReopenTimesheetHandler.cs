using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;
using TimeService.Command.Domain.Events;

namespace TimeService.Command.Application.Handlers;

public class ReopenTimesheetHandler : ICommandHandler<ReopenTimesheetCommand, CommandAcceptedResponse>
{
    private readonly IKafkaProducer _kafkaProducer;

    public ReopenTimesheetHandler(IKafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }

    public async Task<CommandAcceptedResponse> HandleAsync(ReopenTimesheetCommand command, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var evt = new TimesheetStatusChangedEvent
        {
            TimesheetId = command.Id,
            Status = "Draft",
            UpdatedAt = now,
            Comment = command.Comment
        };

        await _kafkaProducer.ProduceAsync(evt, evt, "time.events");
        return new CommandAcceptedResponse(command.Id, evt.Status, "Timesheet reopen request accepted.", evt);
    }
}
