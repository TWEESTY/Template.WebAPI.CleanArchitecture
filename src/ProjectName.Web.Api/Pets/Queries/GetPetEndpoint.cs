using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Application.Pets.Queries;
using ProjectName.Web.Api.Pets.Common;

namespace ProjectName.Web.Api.Pets.Queries;

/// <summary>
/// Represents the endpoint for retrieving a pet by its ID. This endpoint returns the details of a specific pet identified by its unique ID or a not-found response if the pet does not exist.
/// </summary>
internal static class GetPetEndpoint
{
    internal static async Task<Results<Ok<GetPetCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, InternalServerError>> HandleAsync(
        [FromServices] IMediator mediator,
        Guid id)
    {
        Result<GetPetResponse> result = await mediator.Send(new GetPetQuery(id));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(GetPetCommonResponseEndpoint.Create(result.Value));
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
