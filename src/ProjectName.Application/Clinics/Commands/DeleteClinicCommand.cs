using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Clinics.Commands;

/// <summary>
/// Represents a command to delete a clinic in the application.
/// </summary>
/// <param name="Id">The unique identifier of the clinic.</param>
public sealed record DeleteClinicCommand(Guid Id) : ICommand<Result>;

internal sealed class DeleteClinicHandler(IClinicRepository clinicRepository) : ICommandHandler<DeleteClinicCommand, Result>
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
