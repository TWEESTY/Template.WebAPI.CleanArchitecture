using Microsoft.AspNetCore.Http.HttpResults;

namespace ProjectName.Web.Api.Clinics.Commands;

public static class DeleteClinicEndpoint
{
    public static Results<NoContent, NotFound, ValidationProblem, UnauthorizedHttpResult, InternalServerError> HandleAsync(Guid id)
    {
        // Logic to delete the clinic by its ID would go here

        return TypedResults.NoContent();
    }
}