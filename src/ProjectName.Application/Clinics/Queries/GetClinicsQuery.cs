using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Search;

namespace ProjectName.Application.Clinics.Queries;

public sealed record GetClinicsQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetClinicResponse>>>;

public sealed class GetClinicsHandler(IClinicRepository clinicRepository) : IQueryHandler<GetClinicsQuery, Result<List<GetClinicResponse>>>
{
    async ValueTask<Result<List<GetClinicResponse>>> IQueryHandler<GetClinicsQuery, Result<List<GetClinicResponse>>>.Handle(GetClinicsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Clinic> clinics = await clinicRepository.GetAsync(request.SearchParameters, cancellationToken);
        List<GetClinicResponse> responses = clinics
            .Select(clinic => new GetClinicResponse(clinic.Id, clinic.Name, clinic.Address))
            .ToList();

        return Result.Ok(responses);
    }
}
