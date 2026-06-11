using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Clinics.Commands;

public sealed record AddVeterinarianToClinicCommand(Guid ClinicId, Guid VeterinarianId) : ICommand<Result>;

public sealed class AddVeterinarianToClinicHandler(IClinicRepository clinicRepository) : ICommandHandler<AddVeterinarianToClinicCommand, Result>
{
    async ValueTask<Result> ICommandHandler<AddVeterinarianToClinicCommand, Result>.Handle(AddVeterinarianToClinicCommand request, CancellationToken cancellationToken)
    {
        var clinic = await clinicRepository.GetByIdAsync(request.ClinicId, cancellationToken);
        if (clinic is null)
        {
            return Result.Fail(new NotFoundError($"Clinic '{request.ClinicId}' was not found."));
        }

        clinic.AddVeterinarian(request.VeterinarianId);
        await clinicRepository.UpdateAsync(clinic, cancellationToken);

        return Result.Ok();
    }
}
