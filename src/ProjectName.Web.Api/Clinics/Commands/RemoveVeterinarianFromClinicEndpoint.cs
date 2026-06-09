using Microsoft.AspNetCore.Http.HttpResults;

namespace ProjectName.Web.Api.Clinics.Commands;

public static class RemoveVeterinarianFromClinicEndpoint
{
    public static Results<NoContent, NotFound, ValidationProblem, UnauthorizedHttpResult, InternalServerError> HandleAsync(Guid clinicId, Guid veterinarianId)
    {
        // Logic to remove the veterinarian from the clinic would go here

        return TypedResults.NoContent();
    }
}