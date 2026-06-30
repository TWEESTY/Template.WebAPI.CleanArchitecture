namespace ProjectName.Domain.Entities;

/// <summary>
/// Represents the base class for all entities in the domain layer, providing common properties such as Id, CreatedBy, CreatedAt, UpdatedBy, and UpdatedAt.
/// </summary>
public abstract class EntityBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public string CreatedBy { get; protected set; } = null!;
    public DateTimeOffset CreatedAt { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    public DateTimeOffset? UpdatedAt { get; protected set; }
}
