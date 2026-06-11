using ProjectName.Domain.Common.Exceptions;
using ProjectName.Domain.Common.Guards;

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
        Guard.ThrowIf(
            _veterinarianIds.Any(v => v == veterinarianId),
            nameof(VeterinarianIds),
            $"Veterinarian '{veterinarianId}' already exists.");

        _veterinarianIds.Add(veterinarianId);

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveVeterinarian(Guid veterinarianId)
    {
        Guard.ThrowIfNot(
            _veterinarianIds.Any(v => v == veterinarianId),
            nameof(VeterinarianIds),
            $"Veterinarian '{veterinarianId}' does not exist.");

        _veterinarianIds.Remove(veterinarianId);

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string NormalizeName(string name)
    {
        string normalized = name.Trim();
        return Guard.ThrowIfEmptyOrNull(normalized, nameof(Name), "Clinic name cannot be empty.");
    }

    private static string NormalizeAddress(string address)
    {
        string normalized = address.Trim();
        return Guard.ThrowIfEmptyOrNull(normalized, nameof(Address), "Clinic address cannot be empty.");
    }
}