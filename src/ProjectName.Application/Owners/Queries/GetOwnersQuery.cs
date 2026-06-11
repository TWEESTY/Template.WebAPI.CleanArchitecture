using FluentResults;
using Mediator;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Owners.Common;

namespace ProjectName.Application.Owners.Queries;

public sealed record GetOwnersQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetOwnerResponse>>>;

public sealed class GetOwnersHandler(IOwnerRepository repository) : IQueryHandler<GetOwnersQuery, Result<List<GetOwnerResponse>>>
{
    async ValueTask<Result<List<GetOwnerResponse>>> IQueryHandler<GetOwnersQuery, Result<List<GetOwnerResponse>>>.Handle(GetOwnersQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Owner> owners = await repository.GetAsync(request.SearchParameters, cancellationToken);
        List<GetOwnerResponse> responses = owners
            .Select(o => new GetOwnerResponse(o.Id, o.FirstName, o.LastName, o.Email, o.PhoneNumber))
            .ToList();

        return Result.Ok(responses);
    }
}
