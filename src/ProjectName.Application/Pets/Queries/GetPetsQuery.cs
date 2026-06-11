using FluentResults;
using Mediator;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Queries;

public sealed record GetPetsQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetPetResponse>>>;

public sealed class GetPetsHandler(IPetRepository petRepository) : IQueryHandler<GetPetsQuery, Result<List<GetPetResponse>>>
{
    async ValueTask<Result<List<GetPetResponse>>> IQueryHandler<GetPetsQuery, Result<List<GetPetResponse>>>.Handle(GetPetsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Pet> pets = await petRepository.GetAsync(request.SearchParameters, cancellationToken);
        List<GetPetResponse> responses = pets
            .Select(p => new GetPetResponse(
                p.Id,
                p.Name,
                new DateTimeOffset(p.BirthDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero)))
            .ToList();

        return Result.Ok(responses);
    }
}