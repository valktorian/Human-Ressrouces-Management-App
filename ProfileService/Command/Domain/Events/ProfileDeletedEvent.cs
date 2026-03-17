using Infrastructure.Api.Base;

namespace ProfileService.Command.Domain.Events;

public class ProfileDeletedEvent : BaseEvent
{
    public Guid ProfileId { get; }
    public DateTime DeletedAt { get; }

    public ProfileDeletedEvent(Guid profileId, DateTime deletedAt)
    {
        ProfileId = profileId;
        DeletedAt = deletedAt;
    }
}
