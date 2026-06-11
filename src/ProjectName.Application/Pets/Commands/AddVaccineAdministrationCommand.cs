using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Commands;

public sealed record AddVaccineAdministrationCommand(Guid PetId, Guid VaccineId, Guid VeterinarianId, DateOnly AdministrationOn) : ICommand<Result>;

public sealed class AddVaccineAdministrationHandler(IPetRepository petRepository) : ICommandHandler<AddVaccineAdministrationCommand, Result>
{
    async ValueTask<Result> ICommandHandler<AddVaccineAdministrationCommand, Result>.Handle(AddVaccineAdministrationCommand request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.PetId}' was not found."));
        }

        pet.AddVaccineAdministration(request.VaccineId, request.VeterinarianId, request.AdministrationOn);
        await petRepository.UpdateAsync(pet, cancellationToken);

        return Result.Ok();
    }
}
