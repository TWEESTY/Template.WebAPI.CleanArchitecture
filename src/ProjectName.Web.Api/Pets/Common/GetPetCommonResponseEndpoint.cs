using ProjectName.Application.Pets.Common;

namespace ProjectName.Web.Api.Pets.Common;

public record GetPetCommonResponseEndpoint(Guid Id, string Name, DateTimeOffset BirthDate)
{
    public static GetPetCommonResponseEndpoint Create(GetPetResponse response)
    {
        return new GetPetCommonResponseEndpoint(response.Id, response.Name, response.BirthDate);
    }
}