using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Commands;
using ProjectName.Web.Api.Common.Problems;

namespace ProjectName.Web.Api.Veterinarians.Commands;

/// <summary>
/// Represents the endpoint for deleting a veterinarian. This endpoint removes a veterinarian identified by its ID and returns a no-content response on success or appropriate error responses if the veterinarian cannot be deleted.
/// </summary>
internal static class DeleteVeterinarianEndpoint
{
    internal static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        Result result = await mediator.Send(new DeleteVeterinarianCommand(id));

        if (result.IsSuccess)
        {
            return TypedResults.NoContent();
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
}
