using ProjectName.Application.Vaccines.Common;

namespace ProjectName.Web.Api.Vaccines.Common;

public record GetVaccineCommonResponseEndpoint(Guid Id, string Code, string Name)
{
    public static GetVaccineCommonResponseEndpoint Create(GetVaccineResponse response)
    {
        return new GetVaccineCommonResponseEndpoint(response.Id, response.Code, response.Name);
    }
}
