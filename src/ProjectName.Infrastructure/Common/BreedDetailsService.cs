using FluentResults;
using Microsoft.Extensions.Logging;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.DogBreeds.Common;
using ProjectName.Application.DogBreeds.Queries;
using ProjectName.Infrastructure.HttpClients;
using Refit;

namespace ProjectName.Infrastructure.Common;

/// <summary>
/// Represents a service for retrieving dog breed details from an external API, handling errors, and logging relevant information.
/// </summary>
/// <param name="dogBreedApiClient">The API client used to fetch dog breed details.</param>
/// <param name="logger">The logger used to log information and errors.</param>
internal sealed class BreedDetailsService(IDogBreedApiClient dogBreedApiClient, ILogger<BreedDetailsService> logger) : IDogBreedDetailsService
{
    private readonly IDogBreedApiClient _dogBreedApiClient = dogBreedApiClient;
    private readonly ILogger<BreedDetailsService> _logger = logger;
    public async Task<Result<GetDogBreedByIdResponse>> GetByIdAsync(Guid breedId, CancellationToken cancellationToken = default)
    {
        try
        {
            GetDogBreedByIdApiResponse breedResponse = await _dogBreedApiClient.GetBreedByIdAsync(breedId, cancellationToken);
            GetDogBreedByIdData? breed = breedResponse.Data;

            if (breed?.Attributes is null)
            {
                _logger.LogError("Dog API returned an invalid payload for breed id {BreedId}.", breedId);
                return Result.Fail(new UnexpectedError("Dog API returned an invalid payload while retrieving dog breed details."));
            }

            GetDogBreedByIdResponse response = new(
                breed.Id,
                breed.Attributes.Name,
                breed.Attributes.Description,
                breed.Attributes.Life?.Min ?? 0,
                breed.Attributes.Life?.Max ?? 0);

            return Result.Ok(response);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning(ex, "Dog breed with id {BreedId} was not found in external provider.", breedId);
            return Result.Fail(new NotFoundError($"Dog breed '{breedId}' was not found."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching dog breed with id {BreedId}.", breedId);
            return Result.Fail(new UnexpectedError("An unexpected error occurred while retrieving dog breed details."));
        }
    }
}
