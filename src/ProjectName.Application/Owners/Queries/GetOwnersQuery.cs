using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Owners.Common;

namespace ProjectName.Application.Owners.Queries;

/// <summary>
/// Represents a query to retrieve a list of owners based on search parameters in the application.
/// </summary>
/// <param name="SearchParameters">The parameters to filter and sort the owners.</param>
public sealed record GetOwnersQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetOwnerResponse>>>, IHasSearchParameters;

public sealed class GetOwnersQueryValidator : SearchParametersQueryValidator<GetOwnersQuery>;

internal sealed class GetOwnersHandler(IOwnerRepository repository) : IQueryHandler<GetOwnersQuery, Result<List<GetOwnerResponse>>>
{
    async ValueTask<Result<List<GetOwnerResponse>>> IQueryHandler<GetOwnersQuery, Result<List<GetOwnerResponse>>>.Handle(GetOwnersQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<GetOwnerResponse> owners = await repository.GetResponsesAsync(request.SearchParameters, cancellationToken);
        List<GetOwnerResponse> responses = [.. owners];

        return Result.Ok(responses);
    }
}
