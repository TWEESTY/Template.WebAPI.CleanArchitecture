using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Application.Pets.Queries;
using ProjectName.Web.Api.Pets.Common;

namespace ProjectName.Web.Api.Pets.Queries;

/// <summary>
/// Represents the endpoint for retrieving vaccine administration records for a pet. This endpoint returns a list of all vaccine administrations associated with a specific pet.
/// </summary>
internal static class GetPetVaccineAdministrationsEndpoint
{
    internal static async Task<Results<Ok<List<GetPetVaccineAdministrationCommonResponseEndpoint>>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, InternalServerError>> HandleAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id)
    {
        Result<List<GetPetVaccineAdministrationResponse>> result = await mediator.Send(new GetPetVaccineAdministrationsQuery(id));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value.Select(GetPetVaccineAdministrationCommonResponseEndpoint.Create).ToList());
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
