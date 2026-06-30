using ProjectName.Application.Appointments.Common;

namespace ProjectName.Web.Api.Appointments.Common;

public record GetAppointmentCommonResponseEndpoint(
    Guid Id,
    Guid PetId,
    Guid VeterinarianId,
    Guid ClinicId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    string Reason,
    string Status)
{
    internal static GetAppointmentCommonResponseEndpoint Create(GetAppointmentResponse response)
    {
        return new GetAppointmentCommonResponseEndpoint(
            response.Id,
            response.PetId,
            response.VeterinarianId,
            response.ClinicId,
            response.StartAt,
            response.EndAt,
            response.Reason,
            response.Status);
    }
}
