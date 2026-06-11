using FluentResults;
using Mediator;

namespace ProjectName.Application.Clinics.Commands;

public sealed record DeleteClinicCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteClinicHandler : ICommandHandler<DeleteClinicCommand, Result>
{
    ValueTask<Result> ICommandHandler<DeleteClinicCommand, Result>.Handle(DeleteClinicCommand request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Result.Ok());
    }
}
