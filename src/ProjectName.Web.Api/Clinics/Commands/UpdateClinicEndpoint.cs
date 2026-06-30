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
/// Represents the endpoint for updating an existing clinic. This endpoint accepts a clinic ID and updated details, then returns the modified clinic information or appropriate error responses.
/// </summary>
internal static class UpdateClinicEndpoint
{
    internal static async Task<Results<Ok<GetClinicCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id,
        UpdateClinicRequest request)
    {
        Result<GetClinicResponse> result = await mediator.Send(new UpdateClinicCommand(id, request.Name, request.Address));

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

    internal sealed record UpdateClinicRequest(string Name, string Address);
}
