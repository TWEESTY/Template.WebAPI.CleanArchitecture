using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Application.Pets.Common;
using ProjectName.Application.Pets.Queries;
using ProjectName.Web.Api.Common.Search;
using ProjectName.Web.Api.Pets.Common;

namespace ProjectName.Web.Api.Pets.Queries;

/// <summary>
/// Represents the endpoint for retrieving a list of pets. This endpoint returns a paginated or filtered list of all pets in the system based on the provided search parameters.
/// </summary>
internal static class GetPetsEndpoint
{
    internal static async Task<Results<Ok<List<GetPetCommonResponseEndpoint>>, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        [FromServices] IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetPetResponse>> result = await mediator.Send(new GetPetsQuery(
                searchParameters?.ToApplicationSearchParameters()
        ));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value.Select(GetPetCommonResponseEndpoint.Create).ToList());
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
