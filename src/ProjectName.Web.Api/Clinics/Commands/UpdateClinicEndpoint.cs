using Microsoft.AspNetCore.Http.HttpResults;

namespace ProjectName.Web.Api.Clinics.Commands;

public static class UpdateClinicEndpoint
{
    public static Results<Ok<UpdateClinicEndpointResponse>, NotFound, ValidationProblem, UnauthorizedHttpResult, InternalServerError> HandleAsync(Guid id, UpdateClinicRequest request)
    {
        var clinic = new UpdateClinicEndpointResponse(id, request.Name, request.Address);

        return TypedResults.Ok(clinic);
    }

    public record UpdateClinicRequest(string Name, string Address);
    public record UpdateClinicEndpointResponse(Guid Id, string Name, string Address);
}