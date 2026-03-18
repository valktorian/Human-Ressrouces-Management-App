using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;
using TimeService.Command.Domain.Events;

namespace TimeService.Command.Application.Handlers;

public class ApproveLeaveRequestHandler : ICommandHandler<ApproveLeaveRequestCommand, CommandAcceptedResponse>
{
    private readonly IKafkaProducer _kafkaProducer;

    public ApproveLeaveRequestHandler(IKafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }

    public async Task<CommandAcceptedResponse> HandleAsync(ApproveLeaveRequestCommand command, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var evt = new LeaveRequestStatusChangedEvent
        {
            LeaveRequestId = command.Id,
            Status = "Approved",
            DecisionAt = now,
            UpdatedAt = now,
            Comment = command.Comment
        };

        await _kafkaProducer.ProduceAsync(evt, evt, "time.events");
        return new CommandAcceptedResponse(command.Id, evt.Status, "Leave request approve accepted.", evt);
    }
}
