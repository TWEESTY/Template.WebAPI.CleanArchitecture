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
        Code = code;
        Name = name;

        CreatedAt = DateTimeOffset.UtcNow;
    }
}