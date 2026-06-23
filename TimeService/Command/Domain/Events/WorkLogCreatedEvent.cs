using Infrastructure.Api.Base;

namespace TimeService.Command.Domain.Events
{
    public class WorkLogCreatedEvent : BaseEvent
    {
        public Guid Id { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string? Details { get; init; }

        public WorkLogCreatedEvent(Guid id, string? details = null)
        {
            Id = id;
            Details = details;
        }
    }
}
