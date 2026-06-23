using Infrastructure.Api.Base;

namespace EvolutionService.Command.Domain.Events
{
    public class PromotionGrantedEvent : BaseEvent
    {
        public Guid Id { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string? Details { get; init; }

        public PromotionGrantedEvent(Guid id, string? details = null)
        {
            Id = id;
            Details = details;
        }
    }
}
