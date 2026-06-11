using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Application.Veterinarians.Queries;
using ProjectName.Web.Api.Common.Search;
using ProjectName.Web.Api.Veterinarians.Common;

namespace ProjectName.Web.Api.Veterinarians.Queries;

public static class GetVeterinariansEndpoint
{
    public static async Task<Results<Ok<List<GetVeterinarianCommonResponseEndpoint>>, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetVeterinarianResponse>> result = await mediator.Send(new GetVeterinariansQuery(searchParameters?.ToApplicationSearchParameters()));

        if (result.IsSuccess) return TypedResults.Ok(result.Value.Select(GetVeterinarianCommonResponseEndpoint.Create).ToList());
        if (result.HasError<UnauthorizedError>()) return TypedResults.Unauthorized();
        if (result.HasError<ForbiddenError>()) return TypedResults.Forbid();

        return TypedResults.InternalServerError();
    }
}
