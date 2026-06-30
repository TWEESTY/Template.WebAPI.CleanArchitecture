using Refit;
using System.Text.Json.Serialization;

namespace ProjectName.Infrastructure.HttpClients;

/// <summary>
/// HTTP client for retrieving dog breed information from The Dog API.
/// </summary>
public interface IDogBreedApiClient
{
    /// <summary>
    /// Gets a dog breed by ID.
    /// </summary>
    /// <param name="breedId">The breed identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The breed information.</returns>
    [Get("/api/v2/breeds/{breedId}")]
    Task<GetDogBreedByIdApiResponse> GetBreedByIdAsync(Guid breedId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a dog breed response from The Dog API.
/// </summary>
public sealed record GetDogBreedByIdApiResponse(
    GetDogBreedByIdData? Data);

public sealed record GetDogBreedByIdData(
    Guid Id,
    GetDogBreedByIdAttributes? Attributes);

public sealed record GetDogBreedByIdAttributes(
    string Name,
    string Description,
    LifeRange? Life,
    [property: JsonPropertyName("male_weight")] WeightRange? MaleWeight,
    [property: JsonPropertyName("female_weight")] WeightRange? FemaleWeight,
    bool Hypoallergenic);

public sealed record LifeRange(
    int Min,
    int Max);

public sealed record WeightRange(
    int Min,
    int Max);
