using Infrastructure.Api.Base;

namespace AccountService.Command.Domain.Events
{
    public class AccountUpdatedEvent : BaseEvent
    {
        public Guid AccountId { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public DateTime UpdatedAt { get; }

        public AccountUpdatedEvent(Guid accountId, string email, string firstName, string lastName, DateTime updatedAt)
        {
            AccountId = accountId;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            UpdatedAt = updatedAt;
        }
    }
}
