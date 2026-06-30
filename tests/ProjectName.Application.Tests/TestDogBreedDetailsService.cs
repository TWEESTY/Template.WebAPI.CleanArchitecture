using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.DogBreeds.Common;
using ProjectName.Application.DogBreeds.Queries;

namespace ProjectName.Application.Tests;

internal sealed class TestDogBreedDetailsService : IDogBreedDetailsService
{
    private static readonly Guid ExistingBreedId = Guid.Parse("00000000-0000-0000-0000-00000000002a");

    public Task<Result<GetDogBreedByIdResponse>> GetByIdAsync(Guid breedId, CancellationToken cancellationToken = default)
    {
        if (breedId == ExistingBreedId)
        {
            return Task.FromResult(Result.Ok(new GetDogBreedByIdResponse(
                Id: ExistingBreedId,
                Name: "Test Breed",
                Temperament: "Calm",
                LifeSpanMin: 10,
                LifeSpanMax: 14)));
        }

        return Task.FromResult(Result.Fail<GetDogBreedByIdResponse>(new NotFoundError($"Dog breed '{breedId}' was not found.")));
    }
}
