using Infrastructure.Api.Base;

namespace AccountService.Command.Domain.Events
{
    public class AccountPasswordChangedEvent : BaseEvent
    {
        public Guid AccountId { get; }
        public DateTime UpdatedAt { get; }

        public AccountPasswordChangedEvent(Guid accountId, DateTime updatedAt)
        {
            AccountId = accountId;
            UpdatedAt = updatedAt;
        }
    }
}
