using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Common.Search;
using ProjectName.Application.Pets.Common;
using ProjectName.Application.Pets.Queries;
using ProjectName.Web.Api.Pets.Common;
using Microsoft.AspNetCore.Mvc;

namespace ProjectName.Web.Api.Pets.Queries;

public static class GetPetsEndpoint
{
    public static async Task<Results<Ok<List<GetPetCommonResponseEndpoint>>, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        [FromServices] IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetPetResponse>> result = await mediator.Send(new GetPetsQuery(
                searchParameters?.ToApplicationSearchParameters()
        ));

        if(result.IsSuccess)
            return TypedResults.Ok(result.Value.Select(x => GetPetCommonResponseEndpoint.Create(x)).ToList());

        if(result.HasError<UnauthorizedError>())
            return TypedResults.Unauthorized();
        
        if(result.HasError<ForbiddenError>())
            return TypedResults.Forbid();

        return TypedResults.InternalServerError();
    }
}