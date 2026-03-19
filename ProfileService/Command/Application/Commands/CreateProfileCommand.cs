namespace ProfileService.Command.Application.Commands;

public record CreateProfileCommand(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string WorkEmail,
    string? PersonalEmail,
    string? PhoneNumber,
    string? Address,
    DateTime? DateOfBirth,
    string JobTitle,
    string Department,
    Guid? ManagerProfileId,
    string EmploymentType,
    DateTime HireDate,
    string OrganizationRole,
    string EmploymentStatus,
    Guid? AccountId,
    string? ProfilePictureUrl = null);
