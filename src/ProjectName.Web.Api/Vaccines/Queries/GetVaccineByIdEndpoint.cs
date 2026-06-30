using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Application.Vaccines.Queries;
using ProjectName.Web.Api.Vaccines.Common;

namespace ProjectName.Web.Api.Vaccines.Queries;

/// <summary>
/// Represents the endpoint for retrieving a vaccine by its ID. This endpoint returns the details of a specific vaccine identified by its unique ID or a not-found response if the vaccine does not exist.
/// </summary>
internal static class GetVaccineByIdEndpoint
{
    internal static async Task<Results<Ok<GetVaccineCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        Result<GetVaccineResponse> result = await mediator.Send(new GetVaccineByIdQuery(id));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(GetVaccineCommonResponseEndpoint.Create(result.Value));
        }

        if (result.HasError<UnauthorizedError>())
        {
            return TypedResults.Unauthorized();
        }

        if (result.HasError<ForbiddenError>())
        {
            return TypedResults.Forbid();
        }

        if (result.HasError<NotFoundError>())
        {
            return TypedResults.NotFound();
        }

        return TypedResults.InternalServerError();
    }
}
