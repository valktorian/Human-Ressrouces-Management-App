using Infrastructure.Api.Base;

namespace AccountService.Command.Domain.Events;

public class AccountCreatedEvent : BaseEvent
{
    public Guid AccountId { get; set; }
    public string Email { get; set; }

    public AccountCreatedEvent(Guid accountId, string email)
    {
        AccountId = accountId;
        Email = email;
    }
}
