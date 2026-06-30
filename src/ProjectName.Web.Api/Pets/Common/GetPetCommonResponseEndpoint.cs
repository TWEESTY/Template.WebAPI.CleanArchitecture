using ProjectName.Application.Pets.Common;

namespace ProjectName.Web.Api.Pets.Common;

internal sealed record GetPetCommonResponseEndpoint(Guid Id, string Name, DateTimeOffset? BirthDate, int SpecieId)
{
    public static GetPetCommonResponseEndpoint Create(GetPetResponse response)
    {
        return new GetPetCommonResponseEndpoint(response.Id, response.Name, response.BirthDate, response.SpecieId);
    }
}
