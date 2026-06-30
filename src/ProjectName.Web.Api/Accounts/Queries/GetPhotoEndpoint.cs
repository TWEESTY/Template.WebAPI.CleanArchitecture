using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Accounts.Queries;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Web.Api.Accounts.Queries;

/// <summary>
/// Represents the endpoint for retrieving the current authenticated user's profile photo.
/// </summary>
internal static class GetPhotoEndpoint
{
    internal static async Task<Results<FileContentHttpResult, UnauthorizedHttpResult, ForbidHttpResult, NotFound, InternalServerError>> HandleAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        Result<GetPhotoResponse> result = await mediator.Send(new GetPhotoQuery(), cancellationToken);

        if (result.IsSuccess)
        {
            return TypedResults.File(result.Value.Content, result.Value.ContentType);
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

        return TypedResults.InternalServerError();
    }
}