using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Appointments.Commands;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Appointments.Common;
using ProjectName.Web.Api.Common.Problems;

namespace ProjectName.Web.Api.Appointments.Commands;

public static class CreateAppointmentEndpoint
{
    public static async Task<Results<Ok<GetAppointmentCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        CreateAppointmentRequest request)
    {
        Result<GetAppointmentResponse> result = await mediator.Send(new CreateAppointmentCommand(
            request.PetId,
            request.VeterinarianId,
            request.ClinicId,
            request.StartAtUtc,
            request.EndAtUtc,
            request.Reason));

        if (result.IsSuccess) return TypedResults.Ok(GetAppointmentCommonResponseEndpoint.Create(result.Value));
        if (result.HasError<UnauthorizedError>()) return TypedResults.Unauthorized();
        if (result.HasError<ForbiddenError>()) return TypedResults.Forbid();
        if (result.HasError<ValidationError>()) return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());

        return TypedResults.InternalServerError();
    }

    public sealed record CreateAppointmentRequest(
        Guid PetId,
        Guid VeterinarianId,
        Guid ClinicId,
        DateTime StartAtUtc,
        DateTime EndAtUtc,
        string Reason);
}
