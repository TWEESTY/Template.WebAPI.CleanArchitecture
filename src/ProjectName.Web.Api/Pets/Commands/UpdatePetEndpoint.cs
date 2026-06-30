using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Commands;
using ProjectName.Application.Pets.Common;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Web.Api.Pets.Common;

namespace ProjectName.Web.Api.Pets.Commands;

/// <summary>
/// Represents the endpoint for updating an existing pet. This endpoint accepts a pet ID and updated details, then returns the modified pet information or appropriate error responses.
/// </summary>
internal static class UpdatePetEndpoint
{
    internal static async Task<Results<Ok<GetPetCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id,
        [FromBody] UpdatePetEndpointRequest request)
    {
        Result<GetPetResponse> result = await mediator.Send(new UpdatePetCommand(id, request.Name, request.BirthDate));

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

        if (result.HasError<ValidationError>())
        {
            return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());
        }

        return TypedResults.InternalServerError();
    }

    internal sealed record UpdatePetEndpointRequest(string Name, DateTimeOffset BirthDate);
}
