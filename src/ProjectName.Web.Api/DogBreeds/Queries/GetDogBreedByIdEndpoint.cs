using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.DogBreeds.Queries;
using ProjectName.Web.Api.Common.Problems;

namespace ProjectName.Web.Api.DogBreeds.Queries;

/// <summary>
/// Represents the endpoint for retrieving dog breed details by breed ID. This endpoint returns information about a specific dog breed from an external API or a not-found response if the breed does not exist.
/// </summary>
internal static class GetDogBreedByIdEndpoint
{
    internal static async Task<Results<Ok<GetDogBreedByIdCommonResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ValidationProblem, InternalServerError>> HandleAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid breedId,
        CancellationToken cancellationToken)
    {
        Result<GetDogBreedByIdResponse> result = await mediator.Send(new GetDogBreedByIdQuery(breedId), cancellationToken);

        if (result.IsSuccess)
        {
            return TypedResults.Ok(GetDogBreedByIdCommonResponse.Create(result.Value));
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

        if (result.HasError<ValidationError>())
        {
            return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());
        }

        return TypedResults.InternalServerError();
    }
}

internal sealed record GetDogBreedByIdCommonResponse(
    Guid Id,
    string Name,
    string Temperament,
    int LifeSpanMin,
    int LifeSpanMax)
{
    internal static GetDogBreedByIdCommonResponse Create(GetDogBreedByIdResponse response)
    {
        return new GetDogBreedByIdCommonResponse(
            response.Id,
            response.Name,
            response.Temperament,
            response.LifeSpanMin,
            response.LifeSpanMax);
    }
}
