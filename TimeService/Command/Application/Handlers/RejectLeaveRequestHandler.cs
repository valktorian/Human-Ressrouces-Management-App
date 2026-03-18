using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;
using TimeService.Command.Domain.Events;

namespace TimeService.Command.Application.Handlers;

public class RejectLeaveRequestHandler : ICommandHandler<RejectLeaveRequestCommand, CommandAcceptedResponse>
{
    private readonly IKafkaProducer _kafkaProducer;

    public RejectLeaveRequestHandler(IKafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }

    public async Task<CommandAcceptedResponse> HandleAsync(RejectLeaveRequestCommand command, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var evt = new LeaveRequestStatusChangedEvent
        {
            LeaveRequestId = command.Id,
            Status = "Rejected",
            DecisionAt = now,
            UpdatedAt = now,
            Comment = command.Comment
        };

        await _kafkaProducer.ProduceAsync(evt, evt, "time.events");
        return new CommandAcceptedResponse(command.Id, evt.Status, "Leave request reject accepted.", evt);
    }
}
