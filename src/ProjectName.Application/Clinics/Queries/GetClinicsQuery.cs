using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Search;

namespace ProjectName.Application.Clinics.Queries;

public sealed record GetClinicsQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetClinicResponse>>>;

public sealed class GetClinicsHandler : IQueryHandler<GetClinicsQuery, Result<List<GetClinicResponse>>>
{
    ValueTask<Result<List<GetClinicResponse>>> IQueryHandler<GetClinicsQuery, Result<List<GetClinicResponse>>>.Handle(GetClinicsQuery request, CancellationToken cancellationToken)
    {
        var clinics = new List<GetClinicResponse>
        {
            new(Guid.NewGuid(), "Clinic 1", "Address 1"),
            new(Guid.NewGuid(), "Clinic 2", "Address 2")
        };

        return ValueTask.FromResult(Result.Ok(clinics));
    }
}
