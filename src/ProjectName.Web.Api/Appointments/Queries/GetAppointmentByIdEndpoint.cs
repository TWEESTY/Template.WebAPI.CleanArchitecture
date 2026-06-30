using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Appointments.Queries;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Appointments.Common;

namespace ProjectName.Web.Api.Appointments.Queries;

/// <summary>
/// Represents the endpoint for retrieving an appointment by its unique identifier. This endpoint handles the retrieval of an appointment by sending a query to the mediator and returning the appropriate HTTP result based on the outcome of the operation.
/// </summary>
internal static class GetAppointmentByIdEndpoint
{
    internal static async Task<Results<Ok<GetAppointmentCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        Result<GetAppointmentResponse> result = await mediator.Send(new GetAppointmentByIdQuery(id));

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

        return TypedResults.InternalServerError();
    }
}
