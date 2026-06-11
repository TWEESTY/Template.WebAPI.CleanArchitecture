using ProjectName.Application.Appointments.Common;

namespace ProjectName.Web.Api.Appointments.Common;

public record GetAppointmentCommonResponseEndpoint(
    Guid Id,
    Guid PetId,
    Guid VeterinarianId,
    Guid ClinicId,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    string Reason,
    string Status)
{
    public static GetAppointmentCommonResponseEndpoint Create(GetAppointmentResponse response)
    {
        return new GetAppointmentCommonResponseEndpoint(
            response.Id,
            response.PetId,
            response.VeterinarianId,
            response.ClinicId,
            response.StartAtUtc,
            response.EndAtUtc,
            response.Reason,
            response.Status);
    }
}
