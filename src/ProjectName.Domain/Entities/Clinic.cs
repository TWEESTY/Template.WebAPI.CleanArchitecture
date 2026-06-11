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

        Name = NormalizeName(name);
        Address = NormalizeAddress(address);

        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeAddress(string address)
    {
        Address = NormalizeAddress(address);

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeName(string name)
    {
        Name = NormalizeName(name);

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AddVeterinarian(Guid veterinarianId)
    {
        if (_veterinarianIds.Any(v => v == veterinarianId))
        {
            throw new DomainException(
                $"Veterinarian '{veterinarianId}' already exists.", nameof(VeterinarianIds));
        }

        _veterinarianIds.Add(veterinarianId);

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveVeterinarian(Guid veterinarianId)
    {
        if (_veterinarianIds.All(v => v != veterinarianId))
        {
            throw new DomainException(
                $"Veterinarian '{veterinarianId}' does not exist.", nameof(VeterinarianIds));
        }

        _veterinarianIds.Remove(veterinarianId);

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string NormalizeName(string name)
    {
        string normalized = name.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new DomainException("Clinic name cannot be empty.", nameof(Name));
        }

        return normalized;
    }

    private static string NormalizeAddress(string address)
    {
        string normalized = address.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new DomainException("Clinic address cannot be empty.", nameof(Address));
        }

        return normalized;
    }
}