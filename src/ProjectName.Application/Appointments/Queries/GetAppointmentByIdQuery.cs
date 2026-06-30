using FluentResults;
using Mediator;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Appointments.Queries;

/// <summary>
/// Represents a query to retrieve an appointment by its unique identifier in the application.
/// </summary>
/// <param name="Id">The unique identifier of the appointment.</param>
public sealed record GetAppointmentByIdQuery(Guid Id) : IQuery<Result<GetAppointmentResponse>>;

internal sealed class GetAppointmentByIdHandler(IAppointmentRepository repository) : IQueryHandler<GetAppointmentByIdQuery, Result<GetAppointmentResponse>>
{
    async ValueTask<Result<GetAppointmentResponse>> IQueryHandler<GetAppointmentByIdQuery, Result<GetAppointmentResponse>>.Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        Appointment? appointment = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return Result.Fail(new NotFoundError($"Appointment '{request.Id}' was not found."));
        }

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
