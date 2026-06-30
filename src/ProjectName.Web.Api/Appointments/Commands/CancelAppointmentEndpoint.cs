using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Appointments.Commands;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Appointments.Common;
using ProjectName.Web.Api.Common.Problems;

namespace ProjectName.Web.Api.Appointments.Commands;

/// <summary>
/// Represents the endpoint for canceling an appointment. This endpoint handles the cancellation of an appointment by sending a command to the mediator and returning the appropriate HTTP result based on the outcome of the operation.
/// </summary>
internal static class CancelAppointmentEndpoint
{
    internal static async Task<Results<Ok<GetAppointmentCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        Result<GetAppointmentResponse> result = await mediator.Send(new CancelAppointmentCommand(id));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(GetAppointmentCommonResponseEndpoint.Create(result.Value));
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
