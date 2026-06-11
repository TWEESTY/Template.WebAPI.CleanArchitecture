using FluentResults;
using Mediator;
using ProjectName.Application.Appointments.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Appointments.Commands;

public sealed record CreateAppointmentCommand(Guid PetId, Guid VeterinarianId, Guid ClinicId, DateTime StartAtUtc, DateTime EndAtUtc, string Reason)
    : ICommand<Result<GetAppointmentResponse>>;

public sealed class CreateAppointmentHandler(IAppointmentRepository repository) : ICommandHandler<CreateAppointmentCommand, Result<GetAppointmentResponse>>
{
    async ValueTask<Result<GetAppointmentResponse>> ICommandHandler<CreateAppointmentCommand, Result<GetAppointmentResponse>>.Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = new Appointment(request.PetId, request.VeterinarianId, request.ClinicId, request.StartAtUtc, request.EndAtUtc, request.Reason);
        await repository.AddAsync(appointment, cancellationToken);

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
