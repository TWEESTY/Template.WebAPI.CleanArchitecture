using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;

namespace ProjectName.Application.Clinics.Commands;

public sealed record CreateClinicCommand(string Name, string Address) : ICommand<Result<GetClinicResponse>>;

public sealed class CreateClinicHandler : ICommandHandler<CreateClinicCommand, Result<GetClinicResponse>>
{
    ValueTask<Result<GetClinicResponse>> ICommandHandler<CreateClinicCommand, Result<GetClinicResponse>>.Handle(CreateClinicCommand request, CancellationToken cancellationToken)
    {
        var response = new GetClinicResponse(Guid.NewGuid(), request.Name, request.Address);
        return ValueTask.FromResult(Result.Ok(response));
    }
}
