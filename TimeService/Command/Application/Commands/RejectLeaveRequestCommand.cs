namespace TimeService.Command.Application.Commands;

public record RejectLeaveRequestCommand(Guid Id, string? Comment);
