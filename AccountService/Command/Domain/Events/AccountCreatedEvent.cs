using Infrastructure.Api.Base;

namespace AccountService.Command.Domain.Events;

public class AccountCreatedEvent : BaseEvent
{
    public Guid AccountId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Role { get; }
    public bool IsActive { get; }
    public DateTime CreatedAt { get; }

    public AccountCreatedEvent(
        Guid accountId,
        string email,
        string firstName,
        string lastName,
        string role,
        bool isActive,
        DateTime createdAt)
    {
        AccountId = accountId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        IsActive = isActive;
        CreatedAt = createdAt;
    }
}
