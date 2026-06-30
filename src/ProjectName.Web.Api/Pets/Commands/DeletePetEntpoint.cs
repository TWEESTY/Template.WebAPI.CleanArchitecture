using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Commands;
namespace ProjectName.Web.Api.Pets.Commands;

internal static class DeletePetEndpoint
{
    public static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, InternalServerError>> HandleAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id)
    {
        Result result = await mediator.Send(new DeletePetCommand(id));

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

        return TypedResults.InternalServerError();
    }
}
