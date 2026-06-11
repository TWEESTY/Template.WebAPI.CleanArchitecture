namespace ProjectName.Application.Appointments.Common;

public sealed record GetAppointmentResponse(
    Guid Id,
    Guid PetId,
    Guid VeterinarianId,
    Guid ClinicId,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    string Reason,
    string Status);
