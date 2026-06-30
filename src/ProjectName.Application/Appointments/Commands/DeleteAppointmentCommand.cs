using FluentResults;
using Mediator;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Appointments.Commands;

/// <summary>
/// Represents a command to delete an appointment in the application.
/// </summary>
/// <param name="Id">The unique identifier of the appointment to be deleted.</param>
public sealed record DeleteAppointmentCommand(Guid Id) : ICommand<Result>;

internal sealed class DeleteAppointmentHandler(IAppointmentRepository repository) : ICommandHandler<DeleteAppointmentCommand, Result>
{
    async ValueTask<Result> ICommandHandler<DeleteAppointmentCommand, Result>.Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        bool deleted = await repository.DeleteAsync(request.Id, cancellationToken);
        if (!deleted)
        {
            return Result.Fail(new NotFoundError($"Appointment '{request.Id}' was not found."));
        }

        return Result.Ok();
    }
}
