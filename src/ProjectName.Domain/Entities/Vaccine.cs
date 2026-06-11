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
        if (string.IsNullOrWhiteSpace(code))
            throw new Common.Exceptions.DomainException("Code is required.", nameof(Code));

        if (string.IsNullOrWhiteSpace(name))
            throw new Common.Exceptions.DomainException("Name is required.", nameof(Name));

        Code = code.Trim();
        Name = name.Trim();

        UpdatedAt = DateTimeOffset.UtcNow;
    }
}