namespace TimeService.Command.Application.Commands;

public record RejectTimesheetCommand(Guid Id, string? Comment);
