using Infrastructure.Api.Base;

namespace TimeService.Command.Domain.Events;

public sealed class LeaveBalanceAdjustedEvent : BaseEvent
{
    public Guid LeaveBalanceId { get; init; }
    public Guid? AccountId { get; init; }
    public Guid EmployeeId { get; init; }
    public string LeaveType { get; init; } = string.Empty;
    public decimal Available { get; init; }
    public decimal Used { get; init; }
    public decimal Pending { get; init; }
    public decimal Delta { get; init; }
    public string? Reason { get; init; }
    public DateTime UpdatedAt { get; init; }
}
