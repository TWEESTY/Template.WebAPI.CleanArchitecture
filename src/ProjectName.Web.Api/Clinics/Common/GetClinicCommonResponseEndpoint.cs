using ProjectName.Application.Clinics.Common;

namespace ProjectName.Web.Api.Clinics.Common;

public record GetClinicCommonResponseEndpoint(Guid Id, string Name, string Address)
{
    public static GetClinicCommonResponseEndpoint Create(GetClinicResponse response)
    {
        return new GetClinicCommonResponseEndpoint(response.Id, response.Name, response.Address);
    }
}
