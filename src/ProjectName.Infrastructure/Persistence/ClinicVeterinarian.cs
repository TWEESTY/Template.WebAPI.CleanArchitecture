namespace ProjectName.Infrastructure.Persistence;

/// <summary>
/// Represents the relationship between a clinic and a veterinarian, including their respective identifiers. This entity is used to establish the association between clinics and veterinarians in the application's database context.
/// </summary>
public class ClinicVeterinarian
{
    public Guid ClinicId { get; private set; }
    public Guid VeterinarianId { get; private set; }

    private ClinicVeterinarian()
    {
    }

    public ClinicVeterinarian(Guid clinicId, Guid veterinarianId)
    {
        ClinicId = clinicId;
        VeterinarianId = veterinarianId;
    }
}
