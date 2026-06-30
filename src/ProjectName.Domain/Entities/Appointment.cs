using ProjectName.Domain.Common.Guards;
using ProjectName.Domain.Enums;

namespace ProjectName.Domain.Entities;

/// <summary>
/// Represents an appointment entity in the domain layer, encapsulating the details of a scheduled appointment for a pet with a veterinarian at a clinic.
/// </summary>
public class Appointment : EntityBase
{
    public Guid PetId { get; private set; }
    public Guid VeterinarianId { get; private set; }
    public Guid ClinicId { get; private set; }
    public DateTimeOffset StartAt { get; private set; }
    public DateTimeOffset EndAt { get; private set; }
    public string Reason { get; private set; } = null!;
    public AppointmentStatus Status { get; private set; } = null!;

    private Appointment()
    {
    }

    public Appointment(
        Guid petId,
        Guid veterinarianId,
        Guid clinicId,
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        string reason)
    {
        Guard.ThrowIf(endAt <= startAt, nameof(EndAt), "End date must be after start date.");

        Id = Guid.NewGuid();

        PetId = petId;
        VeterinarianId = veterinarianId;
        ClinicId = clinicId;

        StartAt = startAt;
        EndAt = endAt;

        Reason = Guard.ThrowIfEmptyOrNull(reason?.Trim(), nameof(Reason), "Reason is required.");

        Status = AppointmentStatus.Scheduled;

        CreatedAt = TimeProvider.System.GetUtcNow();
    }

    public void Cancel()
    {
        Guard.ThrowIf(Status == AppointmentStatus.Completed, nameof(Status), "Completed appointment cannot be cancelled.");

        Status = AppointmentStatus.Cancelled;

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }

    public void Complete()
    {
        Guard.ThrowIfNot(Status == AppointmentStatus.Scheduled, nameof(Status), "Only scheduled appointments can be completed.");

        Status = AppointmentStatus.Completed;

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }
}
