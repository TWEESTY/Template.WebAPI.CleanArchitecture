using FluentResults;
using Mediator;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Appointments.Commands;

/// <summary>
/// Represents a command to cancel an appointment in the application.
/// </summary>
/// <param name="Id">The unique identifier of the appointment to cancel.</param>
public sealed record CancelAppointmentCommand(Guid Id) : ICommand<Result<GetAppointmentResponse>>;

internal sealed class CancelAppointmentHandler(IAppointmentRepository repository) : ICommandHandler<CancelAppointmentCommand, Result<GetAppointmentResponse>>
{
    async ValueTask<Result<GetAppointmentResponse>> ICommandHandler<CancelAppointmentCommand, Result<GetAppointmentResponse>>.Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        Appointment? appointment = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return Result.Fail(new NotFoundError($"Appointment '{request.Id}' was not found."));
        }

        appointment.Cancel();
        await repository.UpdateAsync(appointment, cancellationToken);

        return Result.Ok(new GetAppointmentResponse(
            appointment.Id,
            appointment.PetId,
            appointment.VeterinarianId,
            appointment.ClinicId,
            appointment.StartAt,
            appointment.EndAt,
            appointment.Reason,
            appointment.Status.Name));
    }
}
