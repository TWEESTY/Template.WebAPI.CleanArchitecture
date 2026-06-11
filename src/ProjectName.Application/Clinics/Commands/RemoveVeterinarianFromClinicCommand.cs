using FluentResults;
using Mediator;

namespace ProjectName.Application.Clinics.Commands;

public sealed record RemoveVeterinarianFromClinicCommand(Guid ClinicId, Guid VeterinarianId) : ICommand<Result>;

public sealed class RemoveVeterinarianFromClinicHandler : ICommandHandler<RemoveVeterinarianFromClinicCommand, Result>
{
    ValueTask<Result> ICommandHandler<RemoveVeterinarianFromClinicCommand, Result>.Handle(RemoveVeterinarianFromClinicCommand request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Result.Ok());
    }
}
