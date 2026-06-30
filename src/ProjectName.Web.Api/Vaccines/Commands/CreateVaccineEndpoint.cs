using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Commands;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Web.Api.Vaccines.Common;

namespace ProjectName.Web.Api.Vaccines.Commands;

/// <summary>
/// Represents the endpoint for creating a new vaccine. This endpoint accepts vaccine details and returns the created vaccine information with a successful response or validation errors if the request is invalid.
/// </summary>
internal static class CreateVaccineEndpoint
{
    internal static async Task<Results<Ok<GetVaccineCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        CreateVaccineRequest request)
    {
        Result<GetVaccineResponse> result = await mediator.Send(new CreateVaccineCommand(request.Code, request.Name));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(GetVaccineCommonResponseEndpoint.Create(result.Value));
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

    internal sealed record CreateVaccineRequest(string Code, string Name);
}
