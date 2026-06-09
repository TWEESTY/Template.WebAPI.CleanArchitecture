using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Web.Api.Common.Search;

namespace ProjectName.Web.Api.Clinics.Queries;

public static class GetClinicsEndpoint
{
    public static Results<Ok<GetClinicsEndpointResponse>, ValidationProblem, UnauthorizedHttpResult, InternalServerError> HandleAsync(SearchParameters searchParameters)
    {
        var clinics = new List<GetClinicsEndpointResponseItem>
        {
            new(Guid.NewGuid(), "Clinic 1", "Address 1"),
            new(Guid.NewGuid(), "Clinic 2", "Address 2")
        };

        return TypedResults.Ok(new GetClinicsEndpointResponse(clinics));
    }


    public record GetClinicsEndpointResponse(List<GetClinicsEndpointResponseItem> Clinics);
    public record GetClinicsEndpointResponseItem(Guid Id, string Name, string Address);
}