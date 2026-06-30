namespace ProjectName.Application.Appointments.Common;

public sealed record GetAppointmentResponse(
    Guid Id,
    Guid PetId,
    Guid VeterinarianId,
    Guid ClinicId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    string Reason,
    string Status);
