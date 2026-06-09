using ProjectName.Domain.Common.Exceptions;

namespace ProjectName.Domain.Entities;

public class Clinic : EntityBase
{
    private readonly List<Guid> _veterinarianIds = [];
    
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public IReadOnlyCollection<Guid> VeterinarianIds => _veterinarianIds;

    private Clinic()
    {
    }

    public Clinic(
        string name,
        string address)
    {
        Id = Guid.NewGuid();

        Name = name;
        Address = address;

        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeAddress(string address)
    {
        Address = address;

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AddVeterinarian(Guid veterinarianId)
    {
        if (_veterinarianIds.Any(v => v == veterinarianId))
        {
            throw new DomainException(
                $"Veterinarian '{veterinarianId}' already exists.");
        }

        _veterinarianIds.Add(veterinarianId);

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveVeterinarian(Guid veterinarianId)
    {
        if (_veterinarianIds.All(v => v != veterinarianId))
        {
            throw new DomainException(
                $"Veterinarian '{veterinarianId}' does not exist.");
        }

        _veterinarianIds.Remove(veterinarianId);

        UpdatedAt = DateTimeOffset.UtcNow;
    }
}