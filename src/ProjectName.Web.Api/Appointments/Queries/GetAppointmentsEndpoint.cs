using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Appointments.Queries;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Appointments.Common;
using ProjectName.Web.Api.Common.Search;

namespace ProjectName.Web.Api.Appointments.Queries;

public static class GetAppointmentsEndpoint
{
    public static async Task<Results<Ok<List<GetAppointmentCommonResponseEndpoint>>, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetAppointmentResponse>> result = await mediator.Send(new GetAppointmentsQuery(searchParameters?.ToApplicationSearchParameters()));

        if (result.IsSuccess) return TypedResults.Ok(result.Value.Select(GetAppointmentCommonResponseEndpoint.Create).ToList());
        if (result.HasError<UnauthorizedError>()) return TypedResults.Unauthorized();
        if (result.HasError<ForbiddenError>()) return TypedResults.Forbid();

        return TypedResults.InternalServerError();
    }
}
