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

public static class UpdatePetEndpoint
{
    public static async Task<Results<Ok<GetPetCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id, 
        [FromBody] UpdatePetEndpointRequest request)
    {
        Result<GetPetResponse> result = await mediator.Send(new UpdatePetCommand(id, request.Name, request.BirthDate));

        if(result.IsSuccess)
            return TypedResults.Ok(GetPetCommonResponseEndpoint.Create(result.Value));

        if(result.HasError<UnauthorizedError>())
            return TypedResults.Unauthorized();
        
        if(result.HasError<ForbiddenError>())
            return TypedResults.Forbid();

        if(result.HasError<NotFoundError>())
            return TypedResults.NotFound();

        if(result.HasError<ValidationError>())
            return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());

        return TypedResults.InternalServerError();
    }

    public record UpdatePetEndpointRequest(string Name, DateTimeOffset BirthDate);
}