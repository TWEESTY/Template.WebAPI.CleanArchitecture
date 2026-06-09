using Microsoft.AspNetCore.Http.HttpResults;

namespace ProjectName.Web.Api.Clinics.Queries;

public static class GetClinicByIdEndpoint
{
    public static Results<Ok<GetClinicByIdEndpointResponse>, NotFound, ValidationProblem, UnauthorizedHttpResult, InternalServerError> HandleAsync(Guid id)
    {
        var clinic = new GetClinicByIdEndpointResponse(id, "Clinic 1", "Address 1");

        return TypedResults.Ok(clinic);
    }

    public record GetClinicByIdEndpointResponse(Guid Id, string Name, string Address);
}