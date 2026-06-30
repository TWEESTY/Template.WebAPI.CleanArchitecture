using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Commands;
using ProjectName.Application.Owners.Common;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Web.Api.Owners.Common;

namespace ProjectName.Web.Api.Owners.Commands;

/// <summary>
/// Represents the endpoint for creating a new owner. This endpoint accepts owner details and returns the created owner information with a successful response or validation errors if the request is invalid.
/// </summary>
internal static class CreateOwnerEndpoint
{
    internal static async Task<Results<Ok<GetOwnerCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        CreateOwnerRequest request)
    {
        Result<GetOwnerResponse> result = await mediator.Send(new CreateOwnerCommand(request.FirstName, request.LastName, request.Email, request.PhoneNumber));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(GetOwnerCommonResponseEndpoint.Create(result.Value));
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

    internal sealed record CreateOwnerRequest(string FirstName, string LastName, string Email, string PhoneNumber);
}
