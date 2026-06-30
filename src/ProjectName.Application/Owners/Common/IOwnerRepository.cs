using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Owners.Common;

/// <summary>
/// Defines the contract for a repository that manages owner entities in the application.
/// </summary>
public interface IOwnerRepository
{
    /// <summary>
    /// Retrieves an owner entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the owner.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The owner entity if found; otherwise, null.</returns>
    Task<Owner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds a new owner entity to the repository.
    /// </summary>
    /// <param name="owner">The owner entity to be added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(Owner owner, CancellationToken cancellationToken = default);
    /// <summary>
    /// Updates an existing owner entity in the repository.
    /// </summary>
    /// <param name="owner">The owner entity to be updated.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(Owner owner, CancellationToken cancellationToken = default);
    /// <summary>
    /// Deletes an owner entity from the repository by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the owner to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A boolean indicating whether the deletion was successful.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Retrieves a list of owner entities based on the specified search parameters.
    /// </summary>
    /// <param name="searchParameters">The parameters to filter and sort the owners.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of owner entities.</returns>
    Task<IReadOnlyList<Owner>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of owner response DTOs based on the specified search parameters.
    /// </summary>
    /// <param name="searchParameters">The parameters to filter and sort the owners.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of owner response DTOs.</returns>
    Task<IReadOnlyList<GetOwnerResponse>> GetResponsesAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);
}
