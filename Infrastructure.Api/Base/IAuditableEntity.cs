namespace Infrastructure.Api.Base;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public void Touch() => UpdatedAt = DateTime.UtcNow;
}
