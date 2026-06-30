using System.Net;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;
using ProjectName.Application.Accounts.Common;
using ProjectName.Application.Accounts.Queries;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Infrastructure.GraphApi;

/// <summary>
/// Retrieves the current authenticated user's photo from Microsoft Graph.
/// </summary>
internal sealed class PhotoService(GraphServiceClient graphServiceClient, ILogger<PhotoService> logger) : IAccountPhotoService
{
    private readonly GraphServiceClient _graphServiceClient = graphServiceClient;
    private readonly ILogger<PhotoService> _logger = logger;

    public async Task<Result<GetPhotoResponse>> GetPhotoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Stream? contentStream = await _graphServiceClient.Me.Photo.Content.GetAsync(cancellationToken: cancellationToken);

            if (contentStream is null)
            {
                return Result.Fail(new NotFoundError("Current user does not have a profile photo."));
            }

            await using (contentStream)
            {
                using MemoryStream memoryStream = new();
                await contentStream.CopyToAsync(memoryStream, cancellationToken);

                return Result.Ok(new GetPhotoResponse(memoryStream.ToArray(), "image/jpeg"));
            }
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.NotFound)
        {
            _logger.LogInformation(ex, "No profile photo found for current user.");
            return Result.Fail(new NotFoundError("Current user does not have a profile photo."));
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning(ex, "Unauthorized while retrieving current user photo from Graph API.");
            return Result.Fail(new UnauthorizedError("Unauthorized to retrieve current user photo from Graph API."));
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Forbidden)
        {
            _logger.LogWarning(ex, "Forbidden while retrieving current user photo from Graph API.");
            return Result.Fail(new ForbiddenError("Forbidden from retrieving current user photo from Graph API."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving current user photo from Graph API.");
            return Result.Fail(new UnexpectedError("An unexpected error occurred while retrieving current user photo."));
        }
    }
}