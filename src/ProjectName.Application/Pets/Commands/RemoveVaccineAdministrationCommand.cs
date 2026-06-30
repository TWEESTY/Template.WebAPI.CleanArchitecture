using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Pets.Commands;

/// <summary>
/// Represents a command to remove a vaccine administration record from a pet in the application.
/// </summary>
/// <param name="PetId">The unique identifier of the pet.</param>
/// <param name="VaccineAdministrationId">The unique identifier of the vaccine administration record to be removed.</param>
public sealed record RemoveVaccineAdministrationCommand(Guid PetId, Guid VaccineAdministrationId) : ICommand<Result>;

internal sealed class RemoveVaccineAdministrationHandler(IPetRepository petRepository) : ICommandHandler<RemoveVaccineAdministrationCommand, Result>
{
    async ValueTask<Result> ICommandHandler<RemoveVaccineAdministrationCommand, Result>.Handle(RemoveVaccineAdministrationCommand request, CancellationToken cancellationToken)
    {
        Pet? pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.PetId}' was not found."));
        }

        pet.RemoveVaccineAdministration(request.VaccineAdministrationId);
        await petRepository.UpdateAsync(pet, cancellationToken);

        return Result.Ok();
    }
}
