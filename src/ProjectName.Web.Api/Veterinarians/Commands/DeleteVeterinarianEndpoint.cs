using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Commands;
using ProjectName.Web.Api.Common.Problems;

namespace ProjectName.Web.Api.Veterinarians.Commands;

public static class DeleteVeterinarianEndpoint
{
    public static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        Result result = await mediator.Send(new DeleteVeterinarianCommand(id));

        if (result.IsSuccess) return TypedResults.NoContent();
        if (result.HasError<UnauthorizedError>()) return TypedResults.Unauthorized();
        if (result.HasError<ForbiddenError>()) return TypedResults.Forbid();
        if (result.HasError<NotFoundError>()) return TypedResults.NotFound();
        if (result.HasError<ValidationError>()) return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());

        return TypedResults.InternalServerError();
    }
}
