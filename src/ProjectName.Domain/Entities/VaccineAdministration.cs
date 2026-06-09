namespace ProjectName.Domain.Entities;

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

        CreatedAt = DateTimeOffset.UtcNow;
    }
}