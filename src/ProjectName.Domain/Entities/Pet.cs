using ProjectName.Domain.Common.Guards;
using ProjectName.Domain.Enums;

namespace ProjectName.Domain.Entities;

/// <summary>
/// Represents a pet entity in the domain layer, encapsulating the details of a pet, including its owner, name, species, birth date, and vaccine administrations.
/// </summary>
public class Pet : EntityBase
{
    private readonly List<VaccineAdministration> _vaccinations = [];


    public Guid OwnerId { get; private set; }
    public string Name { get; private set; } = null!;
    public PetSpecies Species { get; private set; } = null!;
    public DateOnly BirthDate { get; private set; }
    public IReadOnlyCollection<VaccineAdministration> VaccineAdministrations => _vaccinations;


    private Pet()
    {
    }

    public Pet(
        Guid ownerId,
        string name,
        PetSpecies species,
        DateOnly birthDate)
    {
        Id = Guid.NewGuid();

        OwnerId = ownerId;

        Rename(name);

        Species = species;
        BirthDate = birthDate;

        CreatedAt = TimeProvider.System.GetUtcNow();
    }

    public void Rename(string name)
    {
        string normalized = Guard.ThrowIfEmptyOrNull(name?.Trim(), nameof(Name), "Name is required.");
        Guard.ThrowIf(normalized.Length > 100, nameof(Name), "Name is too long.");

        Name = normalized;

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }

    public void TransferOwnership(Guid newOwnerId)
    {
        Guard.ThrowIf(newOwnerId == Guid.Empty, nameof(OwnerId), "Owner is required.");

        OwnerId = newOwnerId;

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }

    public void ChangeBirthDate(DateOnly birthDate)
    {
        BirthDate = birthDate;

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }

    public void AddVaccineAdministration(
        Guid vaccineId,
        Guid veterinarianId,
        DateOnly administeredOn)
    {
        Guard.ThrowIf(
            _vaccinations.Any(v => v.VaccineId == vaccineId),
            nameof(VaccineAdministrations),
            $"Vaccine '{vaccineId}' already exists.");

        _vaccinations.Add(
            new VaccineAdministration(
                Id,
                vaccineId,
                veterinarianId,
                administeredOn));

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }

    public void RemoveVaccineAdministration(Guid vaccineAdministrationId)
    {
        VaccineAdministration? vaccineAdministration = _vaccinations
            .FirstOrDefault(v => v.Id == vaccineAdministrationId);

        VaccineAdministration existingAdministration = Guard.ThrowIfNull(
            vaccineAdministration,
            nameof(VaccineAdministrations),
            "Vaccine administration not found.");

        _ = _vaccinations.Remove(existingAdministration);

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }
}
