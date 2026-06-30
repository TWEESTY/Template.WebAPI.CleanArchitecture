using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Appointments.Queries;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Appointments.Common;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Web.Api.Common.Search;

namespace ProjectName.Web.Api.Appointments.Queries;

/// <summary>
/// Represents the endpoint for retrieving a list of appointments based on search parameters. This endpoint handles the retrieval of appointments by sending a query to the mediator and returning the appropriate HTTP result based on the outcome of the operation.
/// </summary>
internal static class GetAppointmentsEndpoint
{
    internal static async Task<Results<Ok<List<GetAppointmentCommonResponseEndpoint>>, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetAppointmentResponse>> result = await mediator.Send(new GetAppointmentsQuery(searchParameters?.ToApplicationSearchParameters()));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value.Select(GetAppointmentCommonResponseEndpoint.Create).ToList());
        }

        if (result.HasError<UnauthorizedError>())
        {
            return TypedResults.Unauthorized();
        }

        if (result.HasError<ForbiddenError>())
        {
            return TypedResults.Forbid();
        }

        if (result.HasError<ValidationError>())
        {
            return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());
        }

        return TypedResults.InternalServerError();
    }
}
