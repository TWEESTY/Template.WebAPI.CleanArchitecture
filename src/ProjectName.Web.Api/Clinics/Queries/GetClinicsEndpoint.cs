using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Clinics.Queries;
using ProjectName.Application.Common.Errors;
using ProjectName.Web.Api.Clinics.Common;
using ProjectName.Web.Api.Common.Search;

namespace ProjectName.Web.Api.Clinics.Queries;

public static class GetClinicsEndpoint
{
    public static async Task<Results<Ok<List<GetClinicCommonResponseEndpoint>>, UnauthorizedHttpResult, ForbidHttpResult, InternalServerError>> HandleAsync(
        IMediator mediator,
        SearchParameters? searchParameters)
    {
        Result<List<GetClinicResponse>> result = await mediator.Send(new GetClinicsQuery(
            searchParameters?.ToApplicationSearchParameters()
        ));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value.Select(GetClinicCommonResponseEndpoint.Create).ToList());
        }

        if (result.HasError<UnauthorizedError>())
        {
            return TypedResults.Unauthorized();
        }

        if (result.HasError<ForbiddenError>())
        {
            return TypedResults.Forbid();
        }

        return TypedResults.InternalServerError();
    }
}
