using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Commands;

public sealed record TransferPetOwnershipCommand(Guid PetId, Guid NewOwnerId) : ICommand<Result<GetPetResponse>>;

public sealed class TransferPetOwnershipHandler(IPetRepository petRepository) : ICommandHandler<TransferPetOwnershipCommand, Result<GetPetResponse>>
{
    async ValueTask<Result<GetPetResponse>> ICommandHandler<TransferPetOwnershipCommand, Result<GetPetResponse>>.Handle(TransferPetOwnershipCommand request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.PetId}' was not found."));
        }

        pet.TransferOwnership(request.NewOwnerId);
        await petRepository.UpdateAsync(pet, cancellationToken);

        var response = new GetPetResponse(
            pet.Id,
            pet.Name,
            new DateTimeOffset(pet.BirthDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero));

        return Result.Ok(response);
    }
}
