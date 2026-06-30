using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Pets.Queries;

/// <summary>
/// Represents a query to retrieve a pet by its unique identifier in the application.
/// </summary>
/// <param name="Id">The unique identifier of the pet.</param>
public sealed record GetPetQuery(Guid Id) : IQuery<Result<GetPetResponse>>;

internal sealed class GetPetHandler(IPetRepository petRepository) : IQueryHandler<GetPetQuery, Result<GetPetResponse>>
{
    async ValueTask<Result<GetPetResponse>> IQueryHandler<GetPetQuery, Result<GetPetResponse>>.Handle(GetPetQuery request, CancellationToken cancellationToken)
    {
        Pet? pet = await petRepository.GetByIdAsync(request.Id, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.Id}' was not found."));
        }

        GetPetResponse response = new(
            pet.Id,
            pet.Name,
            new DateTimeOffset(pet.BirthDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero),
            pet.Species);

        return Result.Ok(response);
    }
}
