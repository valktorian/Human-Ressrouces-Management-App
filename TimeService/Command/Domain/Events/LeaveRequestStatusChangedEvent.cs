using Infrastructure.Api.Base;

namespace TimeService.Command.Domain.Events;

public sealed class LeaveRequestStatusChangedEvent : BaseEvent
{
    public Guid LeaveRequestId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? SubmittedAt { get; init; }
    public DateTime? DecisionAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string? Comment { get; init; }
}
