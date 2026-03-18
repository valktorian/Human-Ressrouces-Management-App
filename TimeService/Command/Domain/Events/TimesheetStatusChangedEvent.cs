using Infrastructure.Api.Base;

namespace TimeService.Command.Domain.Events;

public sealed class TimesheetStatusChangedEvent : BaseEvent
{
    public Guid TimesheetId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? SubmittedAt { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string? Comment { get; init; }
}
