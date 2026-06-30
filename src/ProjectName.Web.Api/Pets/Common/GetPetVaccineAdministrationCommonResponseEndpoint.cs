using ProjectName.Application.Pets.Common;

namespace ProjectName.Web.Api.Pets.Common;

internal sealed record GetPetVaccineAdministrationCommonResponseEndpoint(
    Guid Id,
    Guid VaccineId,
    Guid VeterinarianId,
    DateOnly AdministrationOn)
{
    public static GetPetVaccineAdministrationCommonResponseEndpoint Create(GetPetVaccineAdministrationResponse response)
    {
        return new GetPetVaccineAdministrationCommonResponseEndpoint(
            response.Id,
            response.VaccineId,
            response.VeterinarianId,
            response.AdministrationOn);
    }
}
