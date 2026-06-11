using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Commands;
using ProjectName.Application.Owners.Common;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Web.Api.Owners.Common;

namespace ProjectName.Web.Api.Owners.Commands;

public static class UpdateOwnerEndpoint
{
    public static async Task<Results<Ok<GetOwnerCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id,
        UpdateOwnerRequest request)
    {
        Result<GetOwnerResponse> result = await mediator.Send(new UpdateOwnerCommand(id, request.FirstName, request.LastName, request.Email, request.PhoneNumber));

        if (result.IsSuccess) return TypedResults.Ok(GetOwnerCommonResponseEndpoint.Create(result.Value));
        if (result.HasError<UnauthorizedError>()) return TypedResults.Unauthorized();
        if (result.HasError<ForbiddenError>()) return TypedResults.Forbid();
        if (result.HasError<NotFoundError>()) return TypedResults.NotFound();
        if (result.HasError<ValidationError>()) return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());

        return TypedResults.InternalServerError();
    }

    public sealed record UpdateOwnerRequest(string FirstName, string LastName, string Email, string PhoneNumber);
}
