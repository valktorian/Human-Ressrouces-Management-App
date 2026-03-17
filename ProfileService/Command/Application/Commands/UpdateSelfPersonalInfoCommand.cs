namespace ProfileService.Command.Application.Commands;

public record UpdateSelfPersonalInfoCommand(
    Guid AccountId,
    string? PersonalEmail,
    string? PhoneNumber,
    string? Address,
    DateTime? DateOfBirth);
