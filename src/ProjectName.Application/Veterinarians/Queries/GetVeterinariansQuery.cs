using FluentResults;
using Mediator;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Veterinarians.Common;

namespace ProjectName.Application.Veterinarians.Queries;

public sealed record GetVeterinariansQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetVeterinarianResponse>>>;

public sealed class GetVeterinariansHandler(IVeterinarianRepository repository) : IQueryHandler<GetVeterinariansQuery, Result<List<GetVeterinarianResponse>>>
{
    async ValueTask<Result<List<GetVeterinarianResponse>>> IQueryHandler<GetVeterinariansQuery, Result<List<GetVeterinarianResponse>>>.Handle(GetVeterinariansQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Veterinarian> veterinarians = await repository.GetAsync(request.SearchParameters, cancellationToken);
        List<GetVeterinarianResponse> responses = veterinarians
            .Select(v => new GetVeterinarianResponse(v.Id, v.FirstName, v.LastName, v.Email, v.LicenseNumber))
            .ToList();

        return Result.Ok(responses);
    }
}
