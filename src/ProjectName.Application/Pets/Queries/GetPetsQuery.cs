using FluentResults;
using Mediator;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Queries;

public sealed record GetPetsQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetPetResponse>>>;

public sealed class GetPetsHandler : IQueryHandler<GetPetsQuery, Result<List<GetPetResponse>>>
{
    ValueTask<Result<List<GetPetResponse>>> IQueryHandler<GetPetsQuery, Result<List<GetPetResponse>>>.Handle(GetPetsQuery request, CancellationToken cancellationToken)
    {
        var pets = new List<GetPetResponse>
        {
            new GetPetResponse(Guid.NewGuid(), "Pet1", DateTimeOffset.Now),
            new GetPetResponse(Guid.NewGuid(), "Pet2", DateTimeOffset.Now)
        };
        return ValueTask.FromResult(Result.Ok(pets));
    }
}