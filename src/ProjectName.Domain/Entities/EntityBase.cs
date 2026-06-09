namespace ProjectName.Domain.Entities;

public abstract class EntityBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public string CreatedBy { get; protected set; } = null!;
    public DateTimeOffset CreatedAt { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    public DateTimeOffset? UpdatedAt { get; protected set; }
}