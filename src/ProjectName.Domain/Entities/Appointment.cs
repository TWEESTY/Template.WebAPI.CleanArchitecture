using ProjectName.Domain.Enums;
using ProjectName.Domain.Common.Exceptions;

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
        if (endAtUtc <= startAtUtc)
            throw new DomainException(
                "End date must be after start date.");

        Id = Guid.NewGuid();

        PetId = petId;
        VeterinarianId = veterinarianId;
        ClinicId = clinicId;

        StartAtUtc = startAtUtc;
        EndAtUtc = endAtUtc;

        Reason = reason;

        Status = AppointmentStatus.Scheduled;

        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status == AppointmentStatus.Completed)
            throw new DomainException(
                "Completed appointment cannot be cancelled.");

        Status = AppointmentStatus.Cancelled;

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new DomainException(
                "Only scheduled appointments can be completed.");

        Status = AppointmentStatus.Completed;

        UpdatedAt = DateTimeOffset.UtcNow;
    }
}