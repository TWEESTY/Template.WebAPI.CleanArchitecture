using Microsoft.AspNetCore.Http.HttpResults;

namespace ProjectName.Web.Api.Accounts.Queries;

public static class GetClaimsEndpoint
{
    public static Results<Ok<GetClaimsEndpointResponse>, UnauthorizedHttpResult, InternalServerError> HandleAsync()
    {
        return TypedResults.Ok(new GetClaimsEndpointResponse(Guid.NewGuid(), "UserName"));
    }


    public record GetClaimsEndpointResponse(Guid Id, string UserName);
}