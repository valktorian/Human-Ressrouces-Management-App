namespace TimeService.Command.Application.Commands;

public record ApproveLeaveRequestCommand(Guid Id, string? Comment);
