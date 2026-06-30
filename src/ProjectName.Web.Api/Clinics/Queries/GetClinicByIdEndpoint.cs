using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Clinics.Queries;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Clinics.Common;

namespace ProjectName.Web.Api.Clinics.Queries;

/// <summary>
/// Represents the endpoint for retrieving a clinic by its ID. This endpoint returns the details of a specific clinic identified by its unique ID or a not-found response if the clinic does not exist.
/// </summary>
internal static class GetClinicByIdEndpoint
{
    internal static async Task<Results<Ok<GetClinicCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        Result<GetClinicResponse> result = await mediator.Send(new GetClinicByIdQuery(id));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(GetClinicCommonResponseEndpoint.Create(result.Value));
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
