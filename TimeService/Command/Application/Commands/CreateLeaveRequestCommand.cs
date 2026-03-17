namespace TimeService.Command.Application.Commands;

public record CreateLeaveRequestCommand(
    Guid EmployeeId,
    string LeaveType,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Reason);
