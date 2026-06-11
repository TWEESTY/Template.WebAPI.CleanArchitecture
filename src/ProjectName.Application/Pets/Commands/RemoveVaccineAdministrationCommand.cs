using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Commands;

public sealed record RemoveVaccineAdministrationCommand(Guid PetId, Guid VaccineAdministrationId) : ICommand<Result>;

public sealed class RemoveVaccineAdministrationHandler(IPetRepository petRepository) : ICommandHandler<RemoveVaccineAdministrationCommand, Result>
{
    async ValueTask<Result> ICommandHandler<RemoveVaccineAdministrationCommand, Result>.Handle(RemoveVaccineAdministrationCommand request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.PetId}' was not found."));
        }

        pet.RemoveVaccineAdministration(request.VaccineAdministrationId);
        await petRepository.UpdateAsync(pet, cancellationToken);

        return Result.Ok();
    }
}
