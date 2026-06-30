using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Appointments.Common;

/// <summary>
/// Represents a repository interface for managing appointments in the application.
/// </summary>
public interface IAppointmentRepository
{
    /// <summary>
    /// Gets an appointment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the appointment.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The appointment if found; otherwise, null.</returns>
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new appointment to the repository.
    /// </summary>
    /// <param name="appointment">The appointment to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing appointment in the repository.
    /// </summary>
    /// <param name="appointment">The appointment to update.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an appointment from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the appointment to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the appointment was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of appointments based on the specified search parameters.
    /// </summary>
    /// <param name="searchParameters">The search parameters to filter appointments.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of appointments.</returns>
    Task<IReadOnlyList<Appointment>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of appointment response DTOs based on the specified search parameters.
    /// </summary>
    /// <param name="searchParameters">The search parameters to filter appointments.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of appointment response DTOs.</returns>
    Task<IReadOnlyList<GetAppointmentResponse>> GetResponsesAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);
}
