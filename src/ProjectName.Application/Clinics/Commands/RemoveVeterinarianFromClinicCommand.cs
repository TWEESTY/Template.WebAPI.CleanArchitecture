using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Clinics.Commands;

/// <summary>
/// Represents a command to remove a veterinarian from a clinic in the application.
/// </summary>
/// <param name="ClinicId">The unique identifier of the clinic.</param>
/// <param name="VeterinarianId">The unique identifier of the veterinarian.</param>
public sealed record RemoveVeterinarianFromClinicCommand(Guid ClinicId, Guid VeterinarianId) : ICommand<Result>;

internal sealed class RemoveVeterinarianFromClinicHandler(IClinicRepository clinicRepository) : ICommandHandler<RemoveVeterinarianFromClinicCommand, Result>
{
    async ValueTask<Result> ICommandHandler<RemoveVeterinarianFromClinicCommand, Result>.Handle(RemoveVeterinarianFromClinicCommand request, CancellationToken cancellationToken)
    {
        Clinic? clinic = await clinicRepository.GetByIdAsync(request.ClinicId, cancellationToken);
        if (clinic is null)
        {
            return Result.Fail(new NotFoundError($"Clinic '{request.ClinicId}' was not found."));
        }

        clinic.RemoveVeterinarian(request.VeterinarianId);
        await clinicRepository.UpdateAsync(clinic, cancellationToken);

        return Result.Ok();
    }
}
