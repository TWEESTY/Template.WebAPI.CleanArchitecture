using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Pets.Commands;

/// <summary>
/// Represents a command to transfer the ownership of a pet to a new owner in the application.
/// </summary>
/// <param name="PetId">The unique identifier of the pet.</param>
/// <param name="NewOwnerId">The unique identifier of the new owner.</param>
public sealed record TransferPetOwnershipCommand(Guid PetId, Guid NewOwnerId) : ICommand<Result<GetPetResponse>>;

internal sealed class TransferPetOwnershipHandler(IPetRepository petRepository) : ICommandHandler<TransferPetOwnershipCommand, Result<GetPetResponse>>
{
    async ValueTask<Result<GetPetResponse>> ICommandHandler<TransferPetOwnershipCommand, Result<GetPetResponse>>.Handle(TransferPetOwnershipCommand request, CancellationToken cancellationToken)
    {
        Pet? pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.PetId}' was not found."));
        }

        pet.TransferOwnership(request.NewOwnerId);
        await petRepository.UpdateAsync(pet, cancellationToken);

        GetPetResponse response = new(
            pet.Id,
            pet.Name,
            new DateTimeOffset(pet.BirthDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero),
            pet.Species);

        return Result.Ok(response);
    }
}
