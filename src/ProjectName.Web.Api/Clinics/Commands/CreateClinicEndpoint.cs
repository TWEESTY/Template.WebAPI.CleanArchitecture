using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Clinics.Commands;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Clinics.Common;
using ProjectName.Web.Api.Common.Problems;

namespace ProjectName.Web.Api.Clinics.Commands;

/// <summary>
/// Represents the endpoint for creating a new clinic. This endpoint accepts clinic details and returns the created clinic information with a successful response or validation errors if the request is invalid.
/// </summary>
internal static class CreateClinicEndpoint
{
    internal static async Task<Results<Ok<GetClinicCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        CreateClinicRequest request)
    {
        Result<GetClinicResponse> result = await mediator.Send(new CreateClinicCommand(request.Name, request.Address));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(GetClinicCommonResponseEndpoint.Create(result.Value));
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

    internal sealed record CreateClinicRequest(string Name, string Address);
}
