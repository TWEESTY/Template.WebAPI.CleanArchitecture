using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Clinics.Commands;

public sealed record CreateClinicCommand(string Name, string Address) : ICommand<Result<GetClinicResponse>>;

public sealed class CreateClinicHandler(IClinicRepository clinicRepository) : ICommandHandler<CreateClinicCommand, Result<GetClinicResponse>>
{
    async ValueTask<Result<GetClinicResponse>> ICommandHandler<CreateClinicCommand, Result<GetClinicResponse>>.Handle(CreateClinicCommand request, CancellationToken cancellationToken)
    {
        var clinic = new Clinic(request.Name, request.Address);
        await clinicRepository.AddAsync(clinic, cancellationToken);

        return Result.Ok(new GetClinicResponse(clinic.Id, clinic.Name, clinic.Address));
    }
}
