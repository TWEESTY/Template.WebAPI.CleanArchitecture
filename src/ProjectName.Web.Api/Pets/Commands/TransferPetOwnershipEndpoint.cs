using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Commands;
using ProjectName.Application.Pets.Common;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Web.Api.Pets.Common;

namespace ProjectName.Web.Api.Pets.Commands;

/// <summary>
/// Represents the endpoint for transferring pet ownership to a new owner. This endpoint changes the owner of a pet and returns the updated pet information or appropriate error responses if the transfer fails.
/// </summary>
internal static class TransferPetOwnershipEndpoint
{
    internal static async Task<Results<Ok<GetPetCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id,
        [FromBody] TransferPetOwnershipRequest request)
    {
        Result<GetPetResponse> result = await mediator.Send(new TransferPetOwnershipCommand(id, request.NewOwnerId));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(GetPetCommonResponseEndpoint.Create(result.Value));
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

    internal sealed record TransferPetOwnershipRequest(Guid NewOwnerId);
}
