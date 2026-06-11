using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Appointments.Commands;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Appointments.Common;
using ProjectName.Web.Api.Common.Problems;

namespace ProjectName.Web.Api.Appointments.Commands;

public static class CancelAppointmentEndpoint
{
    public static async Task<Results<Ok<GetAppointmentCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        Result<GetAppointmentResponse> result = await mediator.Send(new CancelAppointmentCommand(id));

        if (result.IsSuccess) return TypedResults.Ok(GetAppointmentCommonResponseEndpoint.Create(result.Value));
        if (result.HasError<UnauthorizedError>()) return TypedResults.Unauthorized();
        if (result.HasError<ForbiddenError>()) return TypedResults.Forbid();
        if (result.HasError<NotFoundError>()) return TypedResults.NotFound();
        if (result.HasError<ValidationError>()) return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());

        return TypedResults.InternalServerError();
    }
}
