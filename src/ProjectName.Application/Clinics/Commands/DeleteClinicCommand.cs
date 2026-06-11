using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Clinics.Commands;

public sealed record DeleteClinicCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteClinicHandler(IClinicRepository clinicRepository) : ICommandHandler<DeleteClinicCommand, Result>
{
    async ValueTask<Result> ICommandHandler<DeleteClinicCommand, Result>.Handle(DeleteClinicCommand request, CancellationToken cancellationToken)
    {
        bool deleted = await clinicRepository.DeleteAsync(request.Id, cancellationToken);
        if (!deleted)
        {
            return Result.Fail(new NotFoundError($"Clinic '{request.Id}' was not found."));
        }

        return Result.Ok();
    }
}
