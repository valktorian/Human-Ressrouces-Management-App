namespace TimeService.Command.Application.Commands;

public record UpdateLeaveRequestCommand(
    Guid Id,
    string LeaveType,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Reason);
