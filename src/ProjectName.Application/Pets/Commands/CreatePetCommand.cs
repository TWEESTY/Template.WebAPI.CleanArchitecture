using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Domain.Entities;
using ProjectName.Domain.Enums;

namespace ProjectName.Application.Pets.Commands;

public sealed record CreatePetCommand(Guid OwnerId, string Name, int Species, DateTimeOffset BirthDate) : ICommand<Result<GetPetResponse>>;

public sealed class CreatePetHandler(IPetRepository petRepository) : ICommandHandler<CreatePetCommand, Result<GetPetResponse>>
{
    async ValueTask<Result<GetPetResponse>> ICommandHandler<CreatePetCommand, Result<GetPetResponse>>.Handle(CreatePetCommand request, CancellationToken cancellationToken)
    {
        if (!PetSpecies.TryFromValue(request.Species, out PetSpecies? species))
        {
            return Result.Fail(new ValidationError(nameof(request.Species), $"Unsupported species value '{request.Species}'."));
        }

        var pet = new Pet(
            request.OwnerId,
            request.Name,
            species,
            DateOnly.FromDateTime(request.BirthDate.UtcDateTime));

        await petRepository.AddAsync(pet, cancellationToken);

        var response = new GetPetResponse(
            pet.Id,
            pet.Name,
            new DateTimeOffset(pet.BirthDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero));

        return Result.Ok(response);
    }
}
