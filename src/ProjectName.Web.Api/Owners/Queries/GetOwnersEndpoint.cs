using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Common;
using ProjectName.Application.Owners.Queries;
using ProjectName.Web.Api.Common.Search;
using ProjectName.Web.Api.Owners.Common;

namespace ProjectName.Web.Api.Owners.Queries;

public static class GetOwnersEndpoint
{
    public static async Task<Results<Ok<List<GetOwnerCommonResponseEndpoint>>, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetOwnerResponse>> result = await mediator.Send(new GetOwnersQuery(searchParameters?.ToApplicationSearchParameters()));

        if (result.IsSuccess)
            return TypedResults.Ok(result.Value.Select(GetOwnerCommonResponseEndpoint.Create).ToList());
        if (result.HasError<UnauthorizedError>())
            return TypedResults.Unauthorized();
        if (result.HasError<ForbiddenError>())
            return TypedResults.Forbid();

        return TypedResults.InternalServerError();
    }
}
