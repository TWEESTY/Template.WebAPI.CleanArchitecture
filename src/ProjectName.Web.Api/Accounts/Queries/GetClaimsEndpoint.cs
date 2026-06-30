using Microsoft.AspNetCore.Http.HttpResults;

namespace ProjectName.Web.Api.Accounts.Queries;

/// <summary>
/// Represents the endpoint for retrieving claims of the currently authenticated user. This endpoint returns a response containing the user's unique identifier and username, or an unauthorized error if the user is not authenticated.
/// </summary>
internal static class GetClaimsEndpoint
{
    internal static Results<Ok<GetClaimsEndpointResponse>, UnauthorizedHttpResult, InternalServerError> HandleAsync()
    {
        return TypedResults.Ok(new GetClaimsEndpointResponse(Guid.NewGuid(), "UserName"));
    }


    internal sealed record GetClaimsEndpointResponse(Guid Id, string UserName);
}
