using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Appointments.Commands;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Common.Problems;

namespace ProjectName.Web.Api.Appointments.Commands;

/// <summary>
/// Represents the endpoint for deleting an appointment. This endpoint handles the deletion of an appointment by sending a command to the mediator and returning the appropriate HTTP result based on the outcome of the operation.
/// </summary>
internal static class DeleteAppointmentEndpoint
{
    internal static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        Result result = await mediator.Send(new DeleteAppointmentCommand(id));

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
