using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Clinics.Commands;

public sealed record UpdateClinicCommand(Guid Id, string Name, string Address) : ICommand<Result<GetClinicResponse>>;

public sealed class UpdateClinicHandler(IClinicRepository clinicRepository) : ICommandHandler<UpdateClinicCommand, Result<GetClinicResponse>>
{
    async ValueTask<Result<GetClinicResponse>> ICommandHandler<UpdateClinicCommand, Result<GetClinicResponse>>.Handle(UpdateClinicCommand request, CancellationToken cancellationToken)
    {
        var clinic = await clinicRepository.GetByIdAsync(request.Id, cancellationToken);
        if (clinic is null)
        {
            return Result.Fail(new NotFoundError($"Clinic '{request.Id}' was not found."));
        }

        clinic.ChangeName(request.Name);
        clinic.ChangeAddress(request.Address);

        await clinicRepository.UpdateAsync(clinic, cancellationToken);

        return Result.Ok(new GetClinicResponse(clinic.Id, clinic.Name, clinic.Address));
    }
}
