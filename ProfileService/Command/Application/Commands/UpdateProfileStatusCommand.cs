namespace ProfileService.Command.Application.Commands;

public record UpdateProfileStatusCommand(Guid ProfileId, string EmploymentStatus);
