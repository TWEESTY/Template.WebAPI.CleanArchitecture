using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Vaccines.Common;

namespace ProjectName.Application.Vaccines.Queries;

/// <summary>
/// Represents a query to retrieve a list of vaccines based on optional search parameters in the application.
/// </summary>
/// <param name="SearchParameters">The parameters to filter and sort the vaccines.</param>
public sealed record GetVaccinesQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetVaccineResponse>>>, IHasSearchParameters;

public sealed class GetVaccinesQueryValidator : SearchParametersQueryValidator<GetVaccinesQuery>;

internal sealed class GetVaccinesHandler(IVaccineRepository repository) : IQueryHandler<GetVaccinesQuery, Result<List<GetVaccineResponse>>>
{
    async ValueTask<Result<List<GetVaccineResponse>>> IQueryHandler<GetVaccinesQuery, Result<List<GetVaccineResponse>>>.Handle(GetVaccinesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<GetVaccineResponse> vaccines = await repository.GetResponsesAsync(request.SearchParameters, cancellationToken);
        List<GetVaccineResponse> responses = [.. vaccines];

        return Result.Ok(responses);
    }
}
