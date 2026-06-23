using Infrastructure.Api.Base;

namespace EvolutionService.Command.Domain.Events
{
    public class EvaluationUpdatedEvent : BaseEvent
    {
        public Guid Id { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string? Details { get; init; }

        public EvaluationUpdatedEvent(Guid id, string? details = null)
        {
            Id = id;
            Details = details;
        }
    }
}
