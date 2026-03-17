namespace ProfileService.Command.Application.Commands;

public record UpdateProfileCommand(
    Guid ProfileId,
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
    string OrganizationRole);
