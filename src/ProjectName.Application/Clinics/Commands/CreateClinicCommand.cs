using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Clinics.Commands;

/// <summary>
/// Represents a command to create a new clinic in the application.
/// </summary>
/// <param name="Name">The name of the clinic.</param>
/// <param name="Address">The address of the clinic.</param>
public sealed record CreateClinicCommand(string Name, string Address) : ICommand<Result<GetClinicResponse>>, ICreateOrUpdateClinicCommand;

public sealed class CreateClinicCommandValidator : ClinicCommandValidatorBase<CreateClinicCommand>
{
    public CreateClinicCommandValidator()
    {
    }
}

internal sealed class CreateClinicHandler(IClinicRepository clinicRepository) : ICommandHandler<CreateClinicCommand, Result<GetClinicResponse>>
{
    async ValueTask<Result<GetClinicResponse>> ICommandHandler<CreateClinicCommand, Result<GetClinicResponse>>.Handle(CreateClinicCommand request, CancellationToken cancellationToken)
    {
        Clinic clinic = new(request.Name, request.Address);
        await clinicRepository.AddAsync(clinic, cancellationToken);

        return Result.Ok(new GetClinicResponse(clinic.Id, clinic.Name, clinic.Address));
    }
}
