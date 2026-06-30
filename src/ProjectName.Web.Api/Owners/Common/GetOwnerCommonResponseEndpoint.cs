using ProjectName.Application.Owners.Common;

namespace ProjectName.Web.Api.Owners.Common;

internal sealed record GetOwnerCommonResponseEndpoint(Guid Id, string FirstName, string LastName, string Email, string PhoneNumber)
{
    public static GetOwnerCommonResponseEndpoint Create(GetOwnerResponse response)
    {
        return new GetOwnerCommonResponseEndpoint(response.Id, response.FirstName, response.LastName, response.Email, response.PhoneNumber);
    }
}
