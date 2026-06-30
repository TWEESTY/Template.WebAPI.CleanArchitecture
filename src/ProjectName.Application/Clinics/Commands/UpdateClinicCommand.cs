using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Clinics.Commands;

/// <summary>
/// Represents a command to update an existing clinic in the application.
/// </summary>
/// <param name="Id">The unique identifier of the clinic.</param>
/// <param name="Name">The new name of the clinic.</param>
/// <param name="Address">The new address of the clinic.</param>
public sealed record UpdateClinicCommand(Guid Id, string Name, string Address) : ICommand<Result<GetClinicResponse>>, ICreateOrUpdateClinicCommand;

public sealed class UpdateClinicCommandValidator : ClinicCommandValidatorBase<UpdateClinicCommand>
{
    public UpdateClinicCommandValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal sealed class UpdateClinicHandler(IClinicRepository clinicRepository) : ICommandHandler<UpdateClinicCommand, Result<GetClinicResponse>>
{
    async ValueTask<Result<GetClinicResponse>> ICommandHandler<UpdateClinicCommand, Result<GetClinicResponse>>.Handle(UpdateClinicCommand request, CancellationToken cancellationToken)
    {
        Clinic? clinic = await clinicRepository.GetByIdAsync(request.Id, cancellationToken);
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
