using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;

namespace ProjectName.Application.Clinics.Commands;

public sealed record UpdateClinicCommand(Guid Id, string Name, string Address) : ICommand<Result<GetClinicResponse>>;

public sealed class UpdateClinicHandler : ICommandHandler<UpdateClinicCommand, Result<GetClinicResponse>>
{
    ValueTask<Result<GetClinicResponse>> ICommandHandler<UpdateClinicCommand, Result<GetClinicResponse>>.Handle(UpdateClinicCommand request, CancellationToken cancellationToken)
    {
        var response = new GetClinicResponse(request.Id, request.Name, request.Address);
        return ValueTask.FromResult(Result.Ok(response));
    }
}
