namespace ProfileService.Command.Application.Commands;

public record UpdateProfilePictureCommand(
    Guid? ProfileId,
    Guid? AccountId,
    string ProfilePictureUrl);
