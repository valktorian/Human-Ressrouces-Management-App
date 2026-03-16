using Infrastructure.Api.Base;

namespace AccountService.Command.Domain.Events
{
    public class AccountDeletedEvent : BaseEvent
    {
        public Guid AccountId { get; }
        public DateTime DeletedAt { get; }

        public AccountDeletedEvent(Guid accountId, DateTime deletedAt)
        {
            AccountId = accountId;
            DeletedAt = deletedAt;
        }
    }
}
