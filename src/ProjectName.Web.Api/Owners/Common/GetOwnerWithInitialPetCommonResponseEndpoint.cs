using ProjectName.Application.Owners.Common;

namespace ProjectName.Web.Api.Owners.Common;

internal sealed record GetOwnerWithInitialPetCommonResponseEndpoint(
    Guid OwnerId,
    string OwnerFirstName,
    string OwnerLastName,
    Guid PetId,
    string PetName,
    DateTimeOffset PetBirthDate)
{
    public static GetOwnerWithInitialPetCommonResponseEndpoint Create(GetOwnerWithInitialPetResponse response)
    {
        return new GetOwnerWithInitialPetCommonResponseEndpoint(
            response.OwnerId,
            response.OwnerFirstName,
            response.OwnerLastName,
            response.PetId,
            response.PetName,
            response.PetBirthDate);
    }
}