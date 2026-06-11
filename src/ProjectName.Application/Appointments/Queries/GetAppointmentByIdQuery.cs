using FluentResults;
using Mediator;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Appointments.Queries;

public sealed record GetAppointmentByIdQuery(Guid Id) : IQuery<Result<GetAppointmentResponse>>;

public sealed class GetAppointmentByIdHandler(IAppointmentRepository repository) : IQueryHandler<GetAppointmentByIdQuery, Result<GetAppointmentResponse>>
{
    async ValueTask<Result<GetAppointmentResponse>> IQueryHandler<GetAppointmentByIdQuery, Result<GetAppointmentResponse>>.Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return Result.Fail(new NotFoundError($"Appointment '{request.Id}' was not found."));
        }

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
