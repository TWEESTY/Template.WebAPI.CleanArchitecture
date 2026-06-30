using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Application.Vaccines.Queries;
using ProjectName.Web.Api.Common.Search;
using ProjectName.Web.Api.Vaccines.Common;

namespace ProjectName.Web.Api.Vaccines.Queries;

/// <summary>
/// Represents the endpoint for retrieving a list of vaccines. This endpoint returns a paginated or filtered list of all vaccines in the system based on the provided search parameters.
/// </summary>
internal static class GetVaccinesEndpoint
{
    internal static async Task<Results<Ok<List<GetVaccineCommonResponseEndpoint>>, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetVaccineResponse>> result = await mediator.Send(new GetVaccinesQuery(searchParameters?.ToApplicationSearchParameters()));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value.Select(GetVaccineCommonResponseEndpoint.Create).ToList());
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
