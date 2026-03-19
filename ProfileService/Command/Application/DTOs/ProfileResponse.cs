namespace ProfileService.Command.Application.DTOs;

public record ProfileResponse(
    Guid Id,
    Guid? AccountId,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string WorkEmail,
    string? PersonalEmail,
    string? PhoneNumber,
    string? Address,
    string? ProfilePictureUrl,
    DateTime? DateOfBirth,
    string JobTitle,
    string Department,
    Guid? ManagerProfileId,
    string EmploymentType,
    DateTime HireDate,
    string OrganizationRole,
    string EmploymentStatus,
    DateTime CreatedAt,
    DateTime UpdatedAt);
