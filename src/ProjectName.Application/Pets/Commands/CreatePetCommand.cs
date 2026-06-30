using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Domain.Entities;
using ProjectName.Domain.Enums;

namespace ProjectName.Application.Pets.Commands;

/// <summary>
/// Represents a command to create a new pet in the application.
/// </summary>
/// <param name="OwnerId">The unique identifier of the owner of the pet.</param>
/// <param name="Name">The name of the pet.</param>
/// <param name="Species">The species of the pet.</param>
/// <param name="BirthDate">The birth date of the pet.</param>
public sealed record CreatePetCommand(Guid OwnerId, string Name, int Species, DateTimeOffset BirthDate) : ICommand<Result<GetPetResponse>>, ICreateOrUpdatePetCommand;

public sealed class CreatePetCommandValidator : PetCommandValidatorBase<CreatePetCommand>
{
    public CreatePetCommandValidator()
    {
        _ = RuleFor(x => x.OwnerId)
            .NotEmpty();
    }
}

internal sealed class CreatePetHandler(IPetRepository petRepository) : ICommandHandler<CreatePetCommand, Result<GetPetResponse>>
{
    async ValueTask<Result<GetPetResponse>> ICommandHandler<CreatePetCommand, Result<GetPetResponse>>.Handle(CreatePetCommand request, CancellationToken cancellationToken)
    {
        if (!PetSpecies.TryFromValue(request.Species, out PetSpecies? species))
        {
            return Result.Fail(new ValidationError(nameof(request.Species), $"Unsupported species value '{request.Species}'."));
        }

        Pet pet = new(
            request.OwnerId,
            request.Name,
            species,
            DateOnly.FromDateTime(request.BirthDate.UtcDateTime));

        await petRepository.AddAsync(pet, cancellationToken);

        GetPetResponse response = new(
            pet.Id,
            pet.Name,
            new DateTimeOffset(pet.BirthDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero),
            pet.Species);

        return Result.Ok(response);
    }
}
