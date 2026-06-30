using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Pets.Common;

/// <summary>
/// Defines the contract for a repository that manages pet entities in the application.
/// </summary>
public interface IPetRepository
{
    /// <summary>
    /// Retrieves a pet entity by its unique identifier from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the pet.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The pet entity if found; otherwise, null.</returns>
    Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds a new pet entity to the repository.
    /// </summary>
    /// <param name="pet">The pet entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task AddAsync(Pet pet, CancellationToken cancellationToken = default);
    /// <summary>
    /// Updates an existing pet entity in the repository.
    /// </summary>
    /// <param name="pet">The pet entity to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task UpdateAsync(Pet pet, CancellationToken cancellationToken = default);
    /// <summary>
    /// Deletes a pet entity from the repository by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the pet to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the pet was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Retrieves a list of pet entities from the repository based on search parameters.
    /// </summary>
    /// <param name="searchParameters">The search parameters to filter the pets.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of pet entities.</returns>
    Task<IReadOnlyList<Pet>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of pet response DTOs from the repository based on search parameters.
    /// </summary>
    /// <param name="searchParameters">The search parameters to filter the pets.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of pet response DTOs.</returns>
    Task<IReadOnlyList<GetPetResponse>> GetResponsesAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);
}
