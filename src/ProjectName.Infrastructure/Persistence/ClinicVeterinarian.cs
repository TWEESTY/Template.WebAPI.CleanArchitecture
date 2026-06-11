namespace ProjectName.Infrastructure.Persistence;

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
