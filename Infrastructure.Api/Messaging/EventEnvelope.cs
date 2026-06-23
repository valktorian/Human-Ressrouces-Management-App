using Infrastructure.Api.Base;

namespace Infrastructure.Api.Messaging;

public class EventEnvelope
{
    public string EventType { get; set; }
    public object Payload { get; set; }
    public Guid EventId { get; set; }
    public DateTime OccurredAt { get; set; }

    public static EventEnvelope FromEvent(BaseEvent evt, object payload)
    {
        return new EventEnvelope
        {
            EventType = evt.GetType().FullName!,
            Payload = payload,
            EventId = evt.EventId,
            OccurredAt = evt.OccurredAt,
        };
    }
}
