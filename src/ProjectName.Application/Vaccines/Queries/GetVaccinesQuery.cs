using FluentResults;
using Mediator;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Vaccines.Common;

namespace ProjectName.Application.Vaccines.Queries;

public sealed record GetVaccinesQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetVaccineResponse>>>;

public sealed class GetVaccinesHandler(IVaccineRepository repository) : IQueryHandler<GetVaccinesQuery, Result<List<GetVaccineResponse>>>
{
    async ValueTask<Result<List<GetVaccineResponse>>> IQueryHandler<GetVaccinesQuery, Result<List<GetVaccineResponse>>>.Handle(GetVaccinesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Vaccine> vaccines = await repository.GetAsync(request.SearchParameters, cancellationToken);
        List<GetVaccineResponse> responses = vaccines
            .Select(v => new GetVaccineResponse(v.Id, v.Code, v.Name))
            .ToList();

        return Result.Ok(responses);
    }
}
