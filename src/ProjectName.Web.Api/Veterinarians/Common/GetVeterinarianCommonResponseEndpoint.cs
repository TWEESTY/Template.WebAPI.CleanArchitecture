using ProjectName.Application.Veterinarians.Common;

namespace ProjectName.Web.Api.Veterinarians.Common;

public record GetVeterinarianCommonResponseEndpoint(Guid Id, string FirstName, string LastName, string Email, string LicenseNumber)
{
    public static GetVeterinarianCommonResponseEndpoint Create(GetVeterinarianResponse response)
    {
        return new GetVeterinarianCommonResponseEndpoint(response.Id, response.FirstName, response.LastName, response.Email, response.LicenseNumber);
    }
}
