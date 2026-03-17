namespace TimeService.Command.Application.Commands;

public record CreateTimesheetCommand(Guid EmployeeId, DateOnly PeriodStart, DateOnly PeriodEnd);
