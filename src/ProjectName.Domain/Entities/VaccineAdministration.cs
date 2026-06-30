namespace ProjectName.Domain.Entities;

/// <summary>
/// Represents a vaccine administration entity in the domain layer, encapsulating the details of a vaccine administered to a pet by a veterinarian on a specific date.
/// </summary>
public class VaccineAdministration : EntityBase
{
    public Guid PetId { get; private set; }
    public Guid VaccineId { get; private set; }
    public Guid VeterinarianId { get; private set; }
    public DateOnly AdministrationOn { get; private set; }

    private VaccineAdministration()
    {
    }

    public VaccineAdministration(
        Guid petId,
        Guid vaccineId,
        Guid veterinarianId,
        DateOnly administrationOn)
    {
        Id = Guid.NewGuid();
        PetId = petId;
        VaccineId = vaccineId;
        VeterinarianId = veterinarianId;
        AdministrationOn = administrationOn;

        CreatedAt = TimeProvider.System.GetUtcNow();
    }
}
