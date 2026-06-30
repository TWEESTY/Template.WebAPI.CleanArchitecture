using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Clinics.Queries;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Clinics.Common;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Web.Api.Common.Search;

namespace ProjectName.Web.Api.Clinics.Queries;

/// <summary>
/// Represents the endpoint for retrieving a list of clinics. This endpoint returns a paginated or filtered list of all clinics in the system based on the provided search parameters.
/// </summary>
internal static class GetClinicsEndpoint
{
    internal static async Task<Results<Ok<List<GetClinicCommonResponseEndpoint>>, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetClinicResponse>> result = await mediator.Send(new GetClinicsQuery(
            searchParameters?.ToApplicationSearchParameters()
        ));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value.Select(GetClinicCommonResponseEndpoint.Create).ToList());
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
