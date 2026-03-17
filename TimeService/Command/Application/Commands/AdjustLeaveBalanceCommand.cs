namespace TimeService.Command.Application.Commands;

public record AdjustLeaveBalanceCommand(Guid EmployeeId, string LeaveType, decimal Delta, string Reason);
