using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Application.Pets.Queries;
using ProjectName.Web.Api.Pets.Common;
using Microsoft.AspNetCore.Mvc;

namespace ProjectName.Web.Api.Pets.Queries;

public static class GetPetEndpoint
{
    public static async Task<Results<Ok<GetPetCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, InternalServerError>> HandleAsync(
        [FromServices] IMediator mediator,
        Guid id)
    {
        Result<GetPetResponse> result = await mediator.Send(new GetPetQuery(id));

        if(result.IsSuccess)
            return TypedResults.Ok(GetPetCommonResponseEndpoint.Create(result.Value));

        if(result.HasError<UnauthorizedError>())
            return TypedResults.Unauthorized();
        
        if(result.HasError<ForbiddenError>())
            return TypedResults.Forbid();

        if(result.HasError<NotFoundError>())
            return TypedResults.NotFound();
    
        return TypedResults.InternalServerError();
    }
}