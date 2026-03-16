using Infrastructure.Api.Base;

namespace AccountService.Command.Domain.Events
{
    public class AccountRoleUpdatedEvent : BaseEvent
    {
        public Guid AccountId { get; }
        public string Role { get; }
        public DateTime UpdatedAt { get; }

        public AccountRoleUpdatedEvent(Guid accountId, string role, DateTime updatedAt)
        {
            AccountId = accountId;
            Role = role;
            UpdatedAt = updatedAt;
        }
    }
}
