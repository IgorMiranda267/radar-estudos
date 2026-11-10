namespace AIStudyLab.Domain.Entities;

public abstract class AuditableEntity
{
    protected AuditableEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public void MarkUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
