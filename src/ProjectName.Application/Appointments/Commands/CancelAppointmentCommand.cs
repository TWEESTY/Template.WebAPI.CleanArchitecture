using FluentResults;
using Mediator;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Appointments.Commands;

public sealed record CancelAppointmentCommand(Guid Id) : ICommand<Result<GetAppointmentResponse>>;

public sealed class CancelAppointmentHandler(IAppointmentRepository repository) : ICommandHandler<CancelAppointmentCommand, Result<GetAppointmentResponse>>
{
    async ValueTask<Result<GetAppointmentResponse>> ICommandHandler<CancelAppointmentCommand, Result<GetAppointmentResponse>>.Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await repository.GetByIdAsync(request.Id, cancellationToken);
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
            appointment.StartAtUtc,
            appointment.EndAtUtc,
            appointment.Reason,
            appointment.Status.Name));
    }
}
