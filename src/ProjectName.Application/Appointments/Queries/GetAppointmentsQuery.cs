using FluentResults;
using Mediator;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Search;

namespace ProjectName.Application.Appointments.Queries;

public sealed record GetAppointmentsQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetAppointmentResponse>>>;

public sealed class GetAppointmentsHandler(IAppointmentRepository repository) : IQueryHandler<GetAppointmentsQuery, Result<List<GetAppointmentResponse>>>
{
    async ValueTask<Result<List<GetAppointmentResponse>>> IQueryHandler<GetAppointmentsQuery, Result<List<GetAppointmentResponse>>>.Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Appointment> appointments = await repository.GetAsync(request.SearchParameters, cancellationToken);
        List<GetAppointmentResponse> responses = appointments
            .Select(a => new GetAppointmentResponse(
                a.Id,
                a.PetId,
                a.VeterinarianId,
                a.ClinicId,
                a.StartAtUtc,
                a.EndAtUtc,
                a.Reason,
                a.Status.Name))
            .ToList();

        return Result.Ok(responses);
    }
}
