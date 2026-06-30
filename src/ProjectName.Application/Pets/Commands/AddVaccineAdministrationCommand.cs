using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Pets.Commands;

/// <summary>
/// Represents a command to add a vaccine administration record for a pet in the application.
/// </summary>
/// <param name="PetId">The unique identifier of the pet.</param>
/// <param name="VaccineId">The unique identifier of the vaccine.</param>
/// <param name="VeterinarianId">The unique identifier of the veterinarian administering the vaccine.</param>
/// <param name="AdministrationOn">The date when the vaccine was administered.</param>
public sealed record AddVaccineAdministrationCommand(Guid PetId, Guid VaccineId, Guid VeterinarianId, DateOnly AdministrationOn) : ICommand<Result>;

internal sealed class AddVaccineAdministrationHandler(IPetRepository petRepository) : ICommandHandler<AddVaccineAdministrationCommand, Result>
{
    async ValueTask<Result> ICommandHandler<AddVaccineAdministrationCommand, Result>.Handle(AddVaccineAdministrationCommand request, CancellationToken cancellationToken)
    {
        Pet? pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.PetId}' was not found."));
        }

        pet.AddVaccineAdministration(request.VaccineId, request.VeterinarianId, request.AdministrationOn);
        await petRepository.UpdateAsync(pet, cancellationToken);

        return Result.Ok();
    }
}
