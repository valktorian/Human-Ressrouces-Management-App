using Infrastructure.Api.Base;

namespace TimeService.Command.Domain.Events
{
    public class WorkLogApprovedEvent : BaseEvent
    {
        public Guid Id { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string? Details { get; init; }

        public WorkLogApprovedEvent(Guid id, string? details = null)
        {
            Id = id;
            Details = details;
        }
    }
}
