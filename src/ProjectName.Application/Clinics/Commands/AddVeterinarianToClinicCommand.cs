using FluentResults;
using Mediator;

namespace ProjectName.Application.Clinics.Commands;

public sealed record AddVeterinarianToClinicCommand(Guid ClinicId, Guid VeterinarianId) : ICommand<Result>;

public sealed class AddVeterinarianToClinicHandler : ICommandHandler<AddVeterinarianToClinicCommand, Result>
{
    ValueTask<Result> ICommandHandler<AddVeterinarianToClinicCommand, Result>.Handle(AddVeterinarianToClinicCommand request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Result.Ok());
    }
}
