using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Veterinarians.Common;

/// <summary>
/// Defines the contract for a repository that manages veterinarian entities in the application.
/// </summary>
public interface IVeterinarianRepository
{
    /// <summary>
    /// Gets a veterinarian by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the veterinarian.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The veterinarian if found; otherwise, null.</returns>
    Task<Veterinarian?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds a new veterinarian to the repository.
    /// </summary>
    /// <param name="veterinarian">The veterinarian to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddAsync(Veterinarian veterinarian, CancellationToken cancellationToken = default);
    /// <summary>
    /// Updates an existing veterinarian in the repository.
    /// </summary>
    /// <param name="veterinarian">The veterinarian to update.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task UpdateAsync(Veterinarian veterinarian, CancellationToken cancellationToken = default);
    /// <summary>
    /// Deletes a veterinarian from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the veterinarian to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the veterinarian was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Gets a list of veterinarians based on search parameters.
    /// </summary>
    /// <param name="searchParameters">The search parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of veterinarians that match the search parameters.</returns>
    Task<IReadOnlyList<Veterinarian>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of veterinarian response DTOs based on search parameters.
    /// </summary>
    /// <param name="searchParameters">The search parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of veterinarian response DTOs that match the search parameters.</returns>
    Task<IReadOnlyList<GetVeterinarianResponse>> GetResponsesAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);
}
