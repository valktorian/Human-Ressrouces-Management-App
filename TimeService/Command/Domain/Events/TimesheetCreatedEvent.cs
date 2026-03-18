using Infrastructure.Api.Base;

namespace TimeService.Command.Domain.Events;

public sealed class TimesheetCreatedEvent : BaseEvent
{
    public Guid TimesheetId { get; init; }
    public Guid? AccountId { get; init; }
    public Guid EmployeeId { get; init; }
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public decimal TotalHours { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? SubmittedAt { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
