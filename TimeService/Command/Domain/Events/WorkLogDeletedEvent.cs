using Infrastructure.Api.Base;

namespace TimeService.Command.Domain.Events
{
    public class WorkLogDeletedEvent : BaseEvent
    {
        public Guid Id { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string? Details { get; init; }

        public WorkLogDeletedEvent(Guid id, string? details = null)
        {
            Id = id;
            Details = details;
        }
    }
}
