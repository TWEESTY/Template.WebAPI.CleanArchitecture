using FluentResults;
using Mediator;
using ProjectName.Application.Accounts.Common;

namespace ProjectName.Application.Accounts.Queries;

/// <summary>
/// Represents a query to retrieve the authenticated user's profile photo.
/// </summary>
public sealed record GetPhotoQuery : IQuery<Result<GetPhotoResponse>>;

internal sealed class GetPhotoHandler(IAccountPhotoService accountPhotoService) : IQueryHandler<GetPhotoQuery, Result<GetPhotoResponse>>
{
    private readonly IAccountPhotoService _accountPhotoService = accountPhotoService;

    async ValueTask<Result<GetPhotoResponse>> IQueryHandler<GetPhotoQuery, Result<GetPhotoResponse>>.Handle(GetPhotoQuery request, CancellationToken cancellationToken)
    {
        return await _accountPhotoService.GetPhotoAsync(cancellationToken);
    }
}

public sealed record GetPhotoResponse(byte[] Content, string ContentType);