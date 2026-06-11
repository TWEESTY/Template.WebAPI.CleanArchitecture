using ProjectName.Domain.Common.Guards;

namespace ProjectName.Domain.Entities;

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

        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateDetails(
        string code,
        string name)
    {
        Code = Guard.ThrowIfEmptyOrNull(code?.Trim(), nameof(Code), "Code is required.");
        Name = Guard.ThrowIfEmptyOrNull(name?.Trim(), nameof(Name), "Name is required.");

        UpdatedAt = DateTimeOffset.UtcNow;
    }
}