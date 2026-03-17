namespace TimeService.Command.Application.Commands;

public record CreateTimeEntryCommand(
    Guid EmployeeId,
    DateOnly WorkDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string ProjectCode,
    string TaskCode,
    string? Notes);
