using ProjectName.Domain.Common.Exceptions;
using ProjectName.Domain.Enums;

namespace ProjectName.Domain.Entities;

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

        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required.");

        if (name.Length > 100)
            throw new DomainException("Name is too long.");

        Name = name;

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void TransferOwnership(Guid newOwnerId)
    {
        if (newOwnerId == Guid.Empty)
            throw new DomainException("Owner is required.");

        OwnerId = newOwnerId;

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeBirthDate(DateOnly birthDate)
    {
        BirthDate = birthDate;

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AddVaccineAdministration(
        Guid vaccineId,
        Guid veterinarianId,
        DateOnly administeredOn)
    {
        if (_vaccinations.Any(v =>
                v.VaccineId == vaccineId))
        {
            throw new DomainException(
                $"Vaccine '{vaccineId}' already exists.");
        }

        _vaccinations.Add(
            new VaccineAdministration(
                Id,
                vaccineId,
                veterinarianId,
                administeredOn));

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveVaccineAdministration(Guid vaccineAdministrationId)
    {
        var vaccineAdministration = _vaccinations
            .FirstOrDefault(v => v.Id == vaccineAdministrationId);

        if (vaccineAdministration is null)
            throw new DomainException("Vaccine administration not found.");

        _vaccinations.Remove(vaccineAdministration);

        UpdatedAt = DateTimeOffset.UtcNow;
    }
}