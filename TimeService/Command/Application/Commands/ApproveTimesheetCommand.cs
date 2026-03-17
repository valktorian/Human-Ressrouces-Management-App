namespace TimeService.Command.Application.Commands;

public record ApproveTimesheetCommand(Guid Id, string? Comment);
