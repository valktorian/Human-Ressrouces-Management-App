namespace Infrastructure.Api.Messaging;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AggregateType { get; set; } = string.Empty;
    public Guid AggregateId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
}
