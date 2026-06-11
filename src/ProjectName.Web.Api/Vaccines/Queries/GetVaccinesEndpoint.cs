using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Application.Vaccines.Queries;
using ProjectName.Web.Api.Common.Search;
using ProjectName.Web.Api.Vaccines.Common;

namespace ProjectName.Web.Api.Vaccines.Queries;

public static class GetVaccinesEndpoint
{
    public static async Task<Results<Ok<List<GetVaccineCommonResponseEndpoint>>, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetVaccineResponse>> result = await mediator.Send(new GetVaccinesQuery(searchParameters?.ToApplicationSearchParameters()));

        if (result.IsSuccess) return TypedResults.Ok(result.Value.Select(GetVaccineCommonResponseEndpoint.Create).ToList());
        if (result.HasError<UnauthorizedError>()) return TypedResults.Unauthorized();
        if (result.HasError<ForbiddenError>()) return TypedResults.Forbid();

        return TypedResults.InternalServerError();
    }
}
