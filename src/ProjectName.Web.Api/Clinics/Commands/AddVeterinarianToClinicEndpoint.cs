using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Clinics.Commands;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Common.Problems;

namespace ProjectName.Web.Api.Clinics.Commands;

/// <summary>
/// Represents the endpoint for adding a veterinarian to a clinic. This endpoint associates a veterinarian with a specific clinic and returns a no-content response on success or appropriate error responses if the operation fails.
/// </summary>
internal static class AddVeterinarianToClinicEndpoint
{
    internal static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id,
        AddVeterinarianToClinicRequest request)
    {
        Result result = await mediator.Send(new AddVeterinarianToClinicCommand(id, request.VeterinarianId));

        if (result.IsSuccess)
        {
            return TypedResults.NoContent();
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

    internal sealed record AddVeterinarianToClinicRequest(Guid VeterinarianId);
}
