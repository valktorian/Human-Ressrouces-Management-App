namespace TimeService.Command.Application.Commands;

public record UpdateTimeEntryCommand(
    Guid Id,
    DateOnly WorkDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string ProjectCode,
    string TaskCode,
    string? Notes);
