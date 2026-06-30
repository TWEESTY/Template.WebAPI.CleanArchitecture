using FluentResults;
using ProjectName.Application.DogBreeds.Queries;

namespace ProjectName.Application.DogBreeds.Common;

/// <summary>
/// Defines the contract for a service that provides details about dog breeds in the application.
/// </summary>
public interface IDogBreedDetailsService
{
    /// <summary>
    /// Retrieves the details of a dog breed by its unique identifier.
    /// </summary>
    /// <param name="breedId">The unique identifier of the dog breed.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The details of the dog breed.</returns>
    Task<Result<GetDogBreedByIdResponse>> GetByIdAsync(Guid breedId, CancellationToken cancellationToken = default);
}
