namespace TimeService.Command.Application.Commands;

public record CancelLeaveRequestCommand(Guid Id, string? Comment);
