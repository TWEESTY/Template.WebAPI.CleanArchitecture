using FluentResults;
using Mediator;
using ProjectName.Application.DogBreeds.Common;

namespace ProjectName.Application.DogBreeds.Queries;

/// <summary>
/// Represents a query to retrieve a dog breed by its unique identifier in the application.
/// </summary>
/// <param name="BreedId">The unique identifier of the dog breed.</param>
public sealed record GetDogBreedByIdQuery(Guid BreedId) : IQuery<Result<GetDogBreedByIdResponse>>;

internal sealed class GetDogBreedByIdHandler(IDogBreedDetailsService dogBreedDetailsService) : IQueryHandler<GetDogBreedByIdQuery, Result<GetDogBreedByIdResponse>>
{
    private readonly IDogBreedDetailsService _dogBreedDetailsService = dogBreedDetailsService;
    async ValueTask<Result<GetDogBreedByIdResponse>> IQueryHandler<GetDogBreedByIdQuery, Result<GetDogBreedByIdResponse>>.Handle(GetDogBreedByIdQuery request, CancellationToken cancellationToken)
    {
        return await _dogBreedDetailsService.GetByIdAsync(request.BreedId, cancellationToken);
    }
}

public sealed record GetDogBreedByIdResponse(
    Guid Id,
    string Name,
    string Temperament,
    int LifeSpanMin,
    int LifeSpanMax);
