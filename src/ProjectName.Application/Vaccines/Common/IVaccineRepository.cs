using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Vaccines.Common;

/// <summary>
/// Defines the contract for a repository that manages vaccine entities in the application.
/// </summary>
public interface IVaccineRepository
{
    /// <summary>
    /// Retrieves a vaccine by its unique identifier from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the vaccine.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The vaccine entity if found; otherwise, null.</returns>
    Task<Vaccine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds a new vaccine to the repository.
    /// </summary>
    /// <param name="vaccine">The vaccine entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task AddAsync(Vaccine vaccine, CancellationToken cancellationToken = default);
    /// <summary>
    /// Updates an existing vaccine in the repository.
    /// </summary>
    /// <param name="vaccine">The vaccine entity to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task UpdateAsync(Vaccine vaccine, CancellationToken cancellationToken = default);
    /// <summary>
    /// Deletes a vaccine from the repository by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the vaccine to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the vaccine was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Retrieves a list of vaccines from the repository based on search parameters.
    /// </summary>
    /// <param name="searchParameters">The parameters to filter and sort the vaccines.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of vaccines matching the search criteria.</returns>
    Task<IReadOnlyList<Vaccine>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of vaccine response DTOs from the repository based on search parameters.
    /// </summary>
    /// <param name="searchParameters">The parameters to filter and sort the vaccines.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of vaccine response DTOs matching the search criteria.</returns>
    Task<IReadOnlyList<GetVaccineResponse>> GetResponsesAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);
}
