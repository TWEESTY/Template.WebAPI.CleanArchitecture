using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Application.Veterinarians.Queries;
using ProjectName.Web.Api.Common.Search;
using ProjectName.Web.Api.Veterinarians.Common;

namespace ProjectName.Web.Api.Veterinarians.Queries;

/// <summary>
/// Represents the endpoint for retrieving a list of veterinarians. This endpoint returns a paginated or filtered list of all veterinarians in the system based on the provided search parameters.
/// </summary>
internal static class GetVeterinariansEndpoint
{
    internal static async Task<Results<Ok<List<GetVeterinarianCommonResponseEndpoint>>, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetVeterinarianResponse>> result = await mediator.Send(new GetVeterinariansQuery(searchParameters?.ToApplicationSearchParameters()));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value.Select(GetVeterinarianCommonResponseEndpoint.Create).ToList());
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
