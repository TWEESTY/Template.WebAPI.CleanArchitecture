using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Queries;

public sealed record GetPetQuery(Guid id) : IQuery<Result<GetPetResponse>>;

public sealed class GetPetHandler(IPetRepository petRepository) : IQueryHandler<GetPetQuery, Result<GetPetResponse>>
{
    async ValueTask<Result<GetPetResponse>> IQueryHandler<GetPetQuery, Result<GetPetResponse>>.Handle(GetPetQuery request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.id, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.id}' was not found."));
        }

        var response = new GetPetResponse(
            pet.Id,
            pet.Name,
            new DateTimeOffset(pet.BirthDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero));

        return Result.Ok(response);
    }
}
