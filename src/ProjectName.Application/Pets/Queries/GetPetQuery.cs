using FluentResults;
using Mediator;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Queries;

public sealed record GetPetQuery(Guid id) : IQuery<Result<GetPetResponse>>;

public sealed class GetPetHandler : IQueryHandler<GetPetQuery, Result<GetPetResponse>>
{
    ValueTask<Result<GetPetResponse>> IQueryHandler<GetPetQuery, Result<GetPetResponse>>.Handle(GetPetQuery request, CancellationToken cancellationToken)
    {
        var pet = new GetPetResponse(request.id, "PetName", DateTimeOffset.Now);
        return ValueTask.FromResult(Result.Ok(pet));
    }
}