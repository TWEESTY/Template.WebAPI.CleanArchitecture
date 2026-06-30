using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Clinics.Common;

/// <summary>
/// Defines the contract for a repository that manages clinic entities in the application.
/// </summary>
public interface IClinicRepository
{
    /// <summary>
    /// Gets a clinic by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the clinic.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The clinic if found; otherwise, null.</returns>
    Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds a new clinic asynchronously.
    /// </summary>
    /// <param name="clinic">The clinic to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(Clinic clinic, CancellationToken cancellationToken = default);
    /// <summary>
    /// Updates an existing clinic.
    /// </summary>
    /// <param name="clinic">The clinic to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdateAsync(Clinic clinic, CancellationToken cancellationToken = default);
    /// <summary>
    /// Deletes a clinic by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the clinic.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the clinic was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Checks if a clinic exists by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the clinic.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the clinic exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Gets a list of clinics based on search parameters.
    /// </summary>
    /// <param name="searchParameters">The search parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only list of clinics.</returns>
    Task<IReadOnlyList<Clinic>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of clinic response DTOs based on search parameters.
    /// </summary>
    /// <param name="searchParameters">The search parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only list of clinic response DTOs.</returns>
    Task<IReadOnlyList<GetClinicResponse>> GetResponsesAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);
}
