using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Application.Veterinarians.Queries;
using ProjectName.Web.Api.Veterinarians.Common;

namespace ProjectName.Web.Api.Veterinarians.Queries;

public static class GetVeterinarianByIdEndpoint
{
    public static async Task<Results<Ok<GetVeterinarianCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, InternalServerError>> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        Result<GetVeterinarianResponse> result = await mediator.Send(new GetVeterinarianByIdQuery(id));

        if (result.IsSuccess)
            return TypedResults.Ok(GetVeterinarianCommonResponseEndpoint.Create(result.Value));
        if (result.HasError<UnauthorizedError>())
            return TypedResults.Unauthorized();
        if (result.HasError<ForbiddenError>())
            return TypedResults.Forbid();
        if (result.HasError<NotFoundError>())
            return TypedResults.NotFound();

        return TypedResults.InternalServerError();
    }
}
