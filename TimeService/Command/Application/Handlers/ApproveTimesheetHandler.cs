using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;
using TimeService.Command.Domain.Events;

namespace TimeService.Command.Application.Handlers;

public class ApproveTimesheetHandler : ICommandHandler<ApproveTimesheetCommand, CommandAcceptedResponse>
{
    private readonly IKafkaProducer _kafkaProducer;

    public ApproveTimesheetHandler(IKafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }

    public async Task<CommandAcceptedResponse> HandleAsync(ApproveTimesheetCommand command, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var evt = new TimesheetStatusChangedEvent
        {
            TimesheetId = command.Id,
            Status = "Approved",
            ApprovedAt = now,
            UpdatedAt = now,
            Comment = command.Comment
        };

        await _kafkaProducer.ProduceAsync(evt, evt, "time.events");
        return new CommandAcceptedResponse(command.Id, evt.Status, "Timesheet approve request accepted.", evt);
    }
}
