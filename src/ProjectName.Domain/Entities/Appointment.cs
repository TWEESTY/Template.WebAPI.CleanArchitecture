using ProjectName.Domain.Enums;
using ProjectName.Domain.Common.Guards;

namespace ProjectName.Domain.Entities;

public class Appointment : EntityBase
{
    public Guid PetId { get; private set; }
    public Guid VeterinarianId { get; private set; }
    public Guid ClinicId { get; private set; }
    public DateTime StartAtUtc { get; private set; }
    public DateTime EndAtUtc { get; private set; }
    public string Reason { get; private set; } = null!;
    public AppointmentStatus Status { get; private set; } = null!;

    private Appointment()
    {
    }

    public Appointment(
        Guid petId,
        Guid veterinarianId,
        Guid clinicId,
        DateTime startAtUtc,
        DateTime endAtUtc,
        string reason)
    {
        Guard.ThrowIf(endAtUtc <= startAtUtc, nameof(EndAtUtc), "End date must be after start date.");

        Id = Guid.NewGuid();

        PetId = petId;
        VeterinarianId = veterinarianId;
        ClinicId = clinicId;

        StartAtUtc = startAtUtc;
        EndAtUtc = endAtUtc;

        Reason = Guard.ThrowIfEmptyOrNull(reason?.Trim(), nameof(Reason), "Reason is required.");

        Status = AppointmentStatus.Scheduled;

        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        Guard.ThrowIf(Status == AppointmentStatus.Completed, nameof(Status), "Completed appointment cannot be cancelled.");

        Status = AppointmentStatus.Cancelled;

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Complete()
    {
        Guard.ThrowIfNot(Status == AppointmentStatus.Scheduled, nameof(Status), "Only scheduled appointments can be completed.");

        Status = AppointmentStatus.Completed;

        UpdatedAt = DateTimeOffset.UtcNow;
    }
}