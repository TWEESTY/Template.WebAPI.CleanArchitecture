using ProjectName.Domain.Common.Guards;

namespace ProjectName.Domain.Entities;

/// <summary>
/// Represents a clinic entity in the domain layer, encapsulating the details of a veterinary clinic, including its name, address, and associated veterinarians.
/// </summary>
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

#pragma warning disable CA1062 // Validate arguments of public methods
        Name = NormalizeName(name);
        Address = NormalizeAddress(address);
#pragma warning restore CA1062 // Validate arguments of public methods

        CreatedAt = TimeProvider.System.GetUtcNow();
    }

    public void ChangeAddress(string address)
    {
#pragma warning disable CA1062 // Validate arguments of public methods
        Address = NormalizeAddress(address);
#pragma warning restore CA1062 // Validate arguments of public methods

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }

    public void ChangeName(string name)
    {
#pragma warning disable CA1062 // Validate arguments of public methods
        Name = NormalizeName(name);
#pragma warning restore CA1062 // Validate arguments of public methods

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }

    public void AddVeterinarian(Guid veterinarianId)
    {
        Guard.ThrowIf(
            _veterinarianIds.Any(v => v == veterinarianId),
            nameof(VeterinarianIds),
            $"Veterinarian '{veterinarianId}' already exists.");

        _veterinarianIds.Add(veterinarianId);

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }

    public void RemoveVeterinarian(Guid veterinarianId)
    {
        Guard.ThrowIfNot(
            _veterinarianIds.Any(v => v == veterinarianId),
            nameof(VeterinarianIds),
            $"Veterinarian '{veterinarianId}' does not exist.");

        _ = _veterinarianIds.Remove(veterinarianId);

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }

    public void LoadVeterinarianIds(IEnumerable<Guid> veterinarianIds)
    {
        _ = Guard.ThrowIfNull(veterinarianIds, nameof(veterinarianIds), "Veterinarian IDs cannot be null.");

        _veterinarianIds.Clear();
        _veterinarianIds.AddRange(veterinarianIds.Distinct());
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
