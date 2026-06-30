using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Commands;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Web.Api.Veterinarians.Common;

namespace ProjectName.Web.Api.Veterinarians.Commands;

/// <summary>
/// Represents the endpoint for updating an existing veterinarian. This endpoint accepts a veterinarian ID and updated details, then returns the modified veterinarian information or appropriate error responses.
/// </summary>
internal static class UpdateVeterinarianEndpoint
{
    internal static async Task<Results<Ok<GetVeterinarianCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id,
        UpdateVeterinarianRequest request)
    {
        Result<GetVeterinarianResponse> result = await mediator.Send(new UpdateVeterinarianCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.LicenseNumber));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(GetVeterinarianCommonResponseEndpoint.Create(result.Value));
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

    internal sealed record UpdateVeterinarianRequest(string FirstName, string LastName, string Email, string LicenseNumber);
}
