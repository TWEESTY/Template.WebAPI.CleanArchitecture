using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Pets.Commands;

/// <summary>
/// Represents a command to update an existing pet's information in the application.
/// </summary>
/// <param name="Id">The unique identifier of the pet.</param>
/// <param name="Name">The new name of the pet.</param>
/// <param name="BirthDate">The new birth date of the pet.</param>
public sealed record UpdatePetCommand(Guid Id, string Name, DateTimeOffset BirthDate) : ICommand<Result<GetPetResponse>>, ICreateOrUpdatePetCommand;

public sealed class UpdatePetCommandValidator : PetCommandValidatorBase<UpdatePetCommand>
{
    public UpdatePetCommandValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal sealed class UpdatePetHandler(IPetRepository petRepository) : ICommandHandler<UpdatePetCommand, Result<GetPetResponse>>
{
    async ValueTask<Result<GetPetResponse>> ICommandHandler<UpdatePetCommand, Result<GetPetResponse>>.Handle(UpdatePetCommand request, CancellationToken cancellationToken)
    {
        Pet? pet = await petRepository.GetByIdAsync(request.Id, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.Id}' was not found."));
        }

        pet.Rename(request.Name);
        pet.ChangeBirthDate(DateOnly.FromDateTime(request.BirthDate.UtcDateTime));
        await petRepository.UpdateAsync(pet, cancellationToken);

        GetPetResponse response = new(
            pet.Id,
            pet.Name,
            new DateTimeOffset(pet.BirthDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero),
            pet.Species);

        return Result.Ok(response);
    }
}
