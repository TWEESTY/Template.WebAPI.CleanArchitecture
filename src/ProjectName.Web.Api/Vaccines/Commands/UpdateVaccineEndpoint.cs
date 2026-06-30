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
/// Represents the endpoint for updating an existing vaccine. This endpoint accepts a vaccine ID and updated details, then returns the modified vaccine information or appropriate error responses.
/// </summary>
internal static class UpdateVaccineEndpoint
{
    internal static async Task<Results<Ok<GetVaccineCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id,
        UpdateVaccineRequest request)
    {
        Result<GetVaccineResponse> result = await mediator.Send(new UpdateVaccineCommand(id, request.Code, request.Name));

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

    internal sealed record UpdateVaccineRequest(string Code, string Name);
}
