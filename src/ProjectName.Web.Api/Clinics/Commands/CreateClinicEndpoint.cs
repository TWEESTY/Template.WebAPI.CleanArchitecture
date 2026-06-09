using Microsoft.AspNetCore.Http.HttpResults;

namespace ProjectName.Web.Api.Clinics.Commands;

public static class CreateClinicEndpoint
{
    public static Results<Ok<CreateClinicEndpointResponse>, ValidationProblem, UnauthorizedHttpResult, InternalServerError> HandleAsync(CreateClinicRequest request)
    {
        var clinic = new CreateClinicEndpointResponse(Guid.NewGuid(), request.Name, request.Address);

        return TypedResults.Ok(clinic);
    }

    public record CreateClinicRequest(string Name, string Address);
    public record CreateClinicEndpointResponse(Guid Id, string Name, string Address);
}