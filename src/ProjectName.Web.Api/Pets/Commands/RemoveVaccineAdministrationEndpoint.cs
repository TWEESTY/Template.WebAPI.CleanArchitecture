using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Commands;
using ProjectName.Web.Api.Common.Problems;

namespace ProjectName.Web.Api.Pets.Commands;

public static class RemoveVaccineAdministrationEndpoint
{
    public static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id,
        [FromRoute] Guid vaccineAdministrationId)
    {
        Result result = await mediator.Send(new RemoveVaccineAdministrationCommand(id, vaccineAdministrationId));

        if (result.IsSuccess)
            return TypedResults.NoContent();

        if (result.HasError<UnauthorizedError>())
            return TypedResults.Unauthorized();

        if (result.HasError<ForbiddenError>())
            return TypedResults.Forbid();

        if (result.HasError<NotFoundError>())
            return TypedResults.NotFound();

        if (result.HasError<ValidationError>())
            return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());

        return TypedResults.InternalServerError();
    }
}
