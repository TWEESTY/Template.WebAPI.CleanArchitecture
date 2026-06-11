using FluentResults;
using Mediator;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Appointments.Commands;

public sealed record CompleteAppointmentCommand(Guid Id) : ICommand<Result<GetAppointmentResponse>>;

public sealed class CompleteAppointmentHandler(IAppointmentRepository repository) : ICommandHandler<CompleteAppointmentCommand, Result<GetAppointmentResponse>>
{
    async ValueTask<Result<GetAppointmentResponse>> ICommandHandler<CompleteAppointmentCommand, Result<GetAppointmentResponse>>.Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return Result.Fail(new NotFoundError($"Appointment '{request.Id}' was not found."));
        }

        appointment.Complete();
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
