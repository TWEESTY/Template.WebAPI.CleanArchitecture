using ProjectName.Domain.Common.Guards;

namespace ProjectName.Domain.Entities;

/// <summary>
/// Represents a vaccine entity in the domain layer, encapsulating the details of a vaccine, including its code and name.
/// </summary>
public class Vaccine : EntityBase
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;

    private Vaccine()
    {
    }

    public Vaccine(
        string code,
        string name)
    {
        Id = Guid.NewGuid();
        UpdateDetails(code, name);

        CreatedAt = TimeProvider.System.GetUtcNow();
    }

    public void UpdateDetails(
        string code,
        string name)
    {
        Code = Guard.ThrowIfEmptyOrNull(code?.Trim(), nameof(Code), "Code is required.");
        Name = Guard.ThrowIfEmptyOrNull(name?.Trim(), nameof(Name), "Name is required.");

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }
}
