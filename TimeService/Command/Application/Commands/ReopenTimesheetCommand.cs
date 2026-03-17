namespace TimeService.Command.Application.Commands;

public record ReopenTimesheetCommand(Guid Id, string? Comment);
