using FluentResults;
using ProjectName.Application.Accounts.Queries;

namespace ProjectName.Application.Accounts.Common;

/// <summary>
/// Defines contract for retrieving the current authenticated user's photo.
/// </summary>
public interface IAccountPhotoService
{
    /// <summary>
    /// Retrieves the current authenticated user's photo.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The photo content and metadata for the authenticated user.</returns>
    Task<Result<GetPhotoResponse>> GetPhotoAsync(CancellationToken cancellationToken = default);
}