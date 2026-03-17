using ProfileService.Command.Domain;

namespace ProfileService.Command.Application.DTOs;

public static class ProfileMappings
{
    public static ProfileResponse ToResponse(this Profile profile) => new(
        profile.Id,
        profile.AccountId,
        profile.EmployeeNumber,
        profile.FirstName,
        profile.LastName,
        profile.WorkEmail,
        profile.PersonalEmail,
        profile.PhoneNumber,
        profile.Address,
        profile.DateOfBirth,
        profile.JobTitle,
        profile.Department,
        profile.ManagerProfileId,
        profile.EmploymentType,
        profile.HireDate,
        profile.OrganizationRole,
        profile.EmploymentStatus,
        profile.CreatedAt,
        profile.UpdatedAt);
}
