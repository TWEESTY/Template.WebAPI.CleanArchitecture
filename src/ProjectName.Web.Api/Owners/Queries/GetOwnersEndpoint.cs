using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Application.Owners.Common;
using ProjectName.Application.Owners.Queries;
using ProjectName.Web.Api.Common.Search;
using ProjectName.Web.Api.Owners.Common;

namespace ProjectName.Web.Api.Owners.Queries;

/// <summary>
/// Represents the endpoint for retrieving a list of owners. This endpoint returns a paginated or filtered list of all owners in the system based on the provided search parameters.
/// </summary>
internal static class GetOwnersEndpoint
{
    internal static async Task<Results<Ok<List<GetOwnerCommonResponseEndpoint>>, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetOwnerResponse>> result = await mediator.Send(new GetOwnersQuery(searchParameters?.ToApplicationSearchParameters()));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value.Select(GetOwnerCommonResponseEndpoint.Create).ToList());
        }

        if (result.HasError<UnauthorizedError>())
        {
            return TypedResults.Unauthorized();
        }

        if (result.HasError<ForbiddenError>())
        {
            return TypedResults.Forbid();
        }

        if (result.HasError<ValidationError>())
        {
            return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());
        }

        return TypedResults.InternalServerError();
    }
}
