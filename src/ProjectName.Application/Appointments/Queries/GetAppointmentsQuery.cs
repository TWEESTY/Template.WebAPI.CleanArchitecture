using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Search;

namespace ProjectName.Application.Appointments.Queries;

/// <summary>
/// Represents a query to retrieve a list of appointments based on search parameters in the application.
/// </summary>
/// <param name="SearchParameters">The search parameters to filter appointments.</param>
public sealed record GetAppointmentsQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetAppointmentResponse>>>, IHasSearchParameters;

public sealed class GetAppointmentsQueryValidator : SearchParametersQueryValidator<GetAppointmentsQuery>;

internal sealed class GetAppointmentsHandler(IAppointmentRepository repository) : IQueryHandler<GetAppointmentsQuery, Result<List<GetAppointmentResponse>>>
{
    async ValueTask<Result<List<GetAppointmentResponse>>> IQueryHandler<GetAppointmentsQuery, Result<List<GetAppointmentResponse>>>.Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<GetAppointmentResponse> appointments = await repository.GetResponsesAsync(request.SearchParameters, cancellationToken);
        List<GetAppointmentResponse> responses = [.. appointments];

        return Result.Ok(responses);
    }
}
