namespace ProfileService.Command.Application.Commands;

public record UpdateProfileEmploymentCommand(
    Guid ProfileId,
    string JobTitle,
    string Department,
    Guid? ManagerProfileId,
    string EmploymentType,
    DateTime HireDate,
    string OrganizationRole);
