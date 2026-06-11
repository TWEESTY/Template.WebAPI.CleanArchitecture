using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Clinics.Commands;

public sealed record RemoveVeterinarianFromClinicCommand(Guid ClinicId, Guid VeterinarianId) : ICommand<Result>;

public sealed class RemoveVeterinarianFromClinicHandler(IClinicRepository clinicRepository) : ICommandHandler<RemoveVeterinarianFromClinicCommand, Result>
{
    async ValueTask<Result> ICommandHandler<RemoveVeterinarianFromClinicCommand, Result>.Handle(RemoveVeterinarianFromClinicCommand request, CancellationToken cancellationToken)
    {
        var clinic = await clinicRepository.GetByIdAsync(request.ClinicId, cancellationToken);
        if (clinic is null)
        {
            return Result.Fail(new NotFoundError($"Clinic '{request.ClinicId}' was not found."));
        }

        clinic.RemoveVeterinarian(request.VeterinarianId);
        await clinicRepository.UpdateAsync(clinic, cancellationToken);

        return Result.Ok();
    }
}
