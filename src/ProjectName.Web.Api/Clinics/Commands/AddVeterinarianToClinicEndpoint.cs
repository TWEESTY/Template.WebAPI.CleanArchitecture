using Microsoft.AspNetCore.Http.HttpResults;

namespace ProjectName.Web.Api.Clinics.Commands;

public static class AddVeterinarianToClinicEndpoint
{
    public static Results<NoContent, NotFound, ValidationProblem, UnauthorizedHttpResult, InternalServerError> HandleAsync(Guid idClinic, AddVeterinarianToClinicRequest request)
    {
        return TypedResults.NoContent();
    }

    public record AddVeterinarianToClinicRequest(Guid VeterinarianId);
}