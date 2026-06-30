using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Search;

namespace ProjectName.Application.Clinics.Queries;

/// <summary>
/// Represents a query to retrieve a list of clinics based on search parameters in the application.
/// </summary>
/// <param name="SearchParameters">The search parameters for filtering clinics.</param>
public sealed record GetClinicsQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetClinicResponse>>>, IHasSearchParameters;

public sealed class GetClinicsQueryValidator : SearchParametersQueryValidator<GetClinicsQuery>;

internal sealed class GetClinicsHandler(IClinicRepository clinicRepository) : IQueryHandler<GetClinicsQuery, Result<List<GetClinicResponse>>>
{
    async ValueTask<Result<List<GetClinicResponse>>> IQueryHandler<GetClinicsQuery, Result<List<GetClinicResponse>>>.Handle(GetClinicsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<GetClinicResponse> clinics = await clinicRepository.GetResponsesAsync(request.SearchParameters, cancellationToken);
        List<GetClinicResponse> responses = [.. clinics];

        return Result.Ok(responses);
    }
}
