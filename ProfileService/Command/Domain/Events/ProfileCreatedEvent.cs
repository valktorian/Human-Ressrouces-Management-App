using Infrastructure.Api.Base;

namespace ProfileService.Command.Domain.Events;

public class ProfileCreatedEvent : BaseEvent
{
    public Guid ProfileId { get; }
    public Guid? AccountId { get; }
    public string EmployeeNumber { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string WorkEmail { get; }
    public string? PersonalEmail { get; }
    public string? PhoneNumber { get; }
    public string? Address { get; }
    public string? ProfilePictureUrl { get; }
    public DateTime? DateOfBirth { get; }
    public string JobTitle { get; }
    public string Department { get; }
    public Guid? ManagerProfileId { get; }
    public string EmploymentType { get; }
    public DateTime HireDate { get; }
    public string OrganizationRole { get; }
    public string EmploymentStatus { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    public ProfileCreatedEvent(Guid profileId, Guid? accountId, string employeeNumber, string firstName, string lastName, string workEmail, string? personalEmail, string? phoneNumber, string? address, string? profilePictureUrl, DateTime? dateOfBirth, string jobTitle, string department, Guid? managerProfileId, string employmentType, DateTime hireDate, string organizationRole, string employmentStatus, DateTime createdAt, DateTime updatedAt)
    {
        ProfileId = profileId;
        AccountId = accountId;
        EmployeeNumber = employeeNumber;
        FirstName = firstName;
        LastName = lastName;
        WorkEmail = workEmail;
        PersonalEmail = personalEmail;
        PhoneNumber = phoneNumber;
        Address = address;
        ProfilePictureUrl = profilePictureUrl;
        DateOfBirth = dateOfBirth;
        JobTitle = jobTitle;
        Department = department;
        ManagerProfileId = managerProfileId;
        EmploymentType = employmentType;
        HireDate = hireDate;
        OrganizationRole = organizationRole;
        EmploymentStatus = employmentStatus;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
