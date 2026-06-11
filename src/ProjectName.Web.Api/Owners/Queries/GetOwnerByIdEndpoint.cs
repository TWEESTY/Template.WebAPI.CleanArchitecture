using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Common;
using ProjectName.Application.Owners.Queries;
using ProjectName.Web.Api.Owners.Common;

namespace ProjectName.Web.Api.Owners.Queries;

public static class GetOwnerByIdEndpoint
{
    public static async Task<Results<Ok<GetOwnerCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        Result<GetOwnerResponse> result = await mediator.Send(new GetOwnerByIdQuery(id));

        if (result.IsSuccess)
            return TypedResults.Ok(GetOwnerCommonResponseEndpoint.Create(result.Value));
        if (result.HasError<UnauthorizedError>())
            return TypedResults.Unauthorized();
        if (result.HasError<ForbiddenError>())
            return TypedResults.Forbid();
        if (result.HasError<NotFoundError>())
            return TypedResults.NotFound();

        return TypedResults.InternalServerError();
    }
}
