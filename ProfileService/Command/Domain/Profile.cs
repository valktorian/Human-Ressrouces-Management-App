using Infrastructure.Api.Base;

namespace ProfileService.Command.Domain;

public class Profile : BaseEntity
{
    public Guid? AccountId { get; private set; }
    public string EmployeeNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string WorkEmail { get; private set; } = string.Empty;
    public string? PersonalEmail { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string JobTitle { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public Guid? ManagerProfileId { get; private set; }
    public string EmploymentType { get; private set; } = string.Empty;
    public DateTime HireDate { get; private set; }
    public string OrganizationRole { get; private set; } = string.Empty;
    public string EmploymentStatus { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Profile()
    {
    }

    public static Profile Create(Guid? accountId, string employeeNumber, string firstName, string lastName, string workEmail, string? personalEmail, string? phoneNumber, string? address, string? profilePictureUrl, DateTime? dateOfBirth, string jobTitle, string department, Guid? managerProfileId, string employmentType, DateTime hireDate, string organizationRole, string employmentStatus)
    {
        var now = DateTime.UtcNow;

        return new Profile
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            EmployeeNumber = employeeNumber.Trim().ToUpperInvariant(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            WorkEmail = workEmail.Trim().ToLowerInvariant(),
            PersonalEmail = NormalizeOptionalEmail(personalEmail),
            PhoneNumber = NormalizeOptional(phoneNumber),
            Address = NormalizeOptional(address),
            ProfilePictureUrl = NormalizeOptional(profilePictureUrl),
            DateOfBirth = dateOfBirth,
            JobTitle = jobTitle.Trim(),
            Department = department.Trim(),
            ManagerProfileId = managerProfileId,
            EmploymentType = employmentType.Trim(),
            HireDate = DateTime.SpecifyKind(hireDate, DateTimeKind.Utc),
            OrganizationRole = organizationRole.Trim(),
            EmploymentStatus = employmentStatus.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    public void UpdateCore(string employeeNumber, string firstName, string lastName, string workEmail, string? personalEmail, string? phoneNumber, string? address, DateTime? dateOfBirth, string jobTitle, string department, Guid? managerProfileId, string employmentType, DateTime hireDate, string organizationRole)
    {
        EmployeeNumber = employeeNumber.Trim().ToUpperInvariant();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        WorkEmail = workEmail.Trim().ToLowerInvariant();
        PersonalEmail = NormalizeOptionalEmail(personalEmail);
        PhoneNumber = NormalizeOptional(phoneNumber);
        Address = NormalizeOptional(address);
        DateOfBirth = dateOfBirth;
        JobTitle = jobTitle.Trim();
        Department = department.Trim();
        ManagerProfileId = managerProfileId;
        EmploymentType = employmentType.Trim();
        HireDate = DateTime.SpecifyKind(hireDate, DateTimeKind.Utc);
        OrganizationRole = organizationRole.Trim();
        Touch();
    }

    public void UpdateEmployment(string jobTitle, string department, Guid? managerProfileId, string employmentType, DateTime hireDate, string organizationRole)
    {
        JobTitle = jobTitle.Trim();
        Department = department.Trim();
        ManagerProfileId = managerProfileId;
        EmploymentType = employmentType.Trim();
        HireDate = DateTime.SpecifyKind(hireDate, DateTimeKind.Utc);
        OrganizationRole = organizationRole.Trim();
        Touch();
    }

    public void UpdateStatus(string employmentStatus)
    {
        EmploymentStatus = employmentStatus.Trim();
        Touch();
    }

    public void LinkAccount(Guid accountId)
    {
        AccountId = accountId;
        Touch();
    }

    public void UpdatePersonalInfo(string? personalEmail, string? phoneNumber, string? address, DateTime? dateOfBirth)
    {
        PersonalEmail = NormalizeOptionalEmail(personalEmail);
        PhoneNumber = NormalizeOptional(phoneNumber);
        Address = NormalizeOptional(address);
        DateOfBirth = dateOfBirth;
        Touch();
    }

    public void UpdateProfilePicture(string profilePictureUrl)
    {
        ProfilePictureUrl = NormalizeOptional(profilePictureUrl);
        Touch();
    }

    private void Touch() => UpdatedAt = DateTime.UtcNow;
    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string? NormalizeOptionalEmail(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
}
