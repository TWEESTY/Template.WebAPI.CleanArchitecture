using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Veterinarians.Common;

namespace ProjectName.Application.Veterinarians.Queries;

/// <summary>
/// Represents a query to retrieve a list of veterinarians based on optional search parameters in the application.
/// </summary>
/// <param name="SearchParameters">The search parameters to filter the list of veterinarians.</param>
public sealed record GetVeterinariansQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetVeterinarianResponse>>>, IHasSearchParameters;

public sealed class GetVeterinariansQueryValidator : SearchParametersQueryValidator<GetVeterinariansQuery>;

internal sealed class GetVeterinariansHandler(IVeterinarianRepository repository) : IQueryHandler<GetVeterinariansQuery, Result<List<GetVeterinarianResponse>>>
{
    async ValueTask<Result<List<GetVeterinarianResponse>>> IQueryHandler<GetVeterinariansQuery, Result<List<GetVeterinarianResponse>>>.Handle(GetVeterinariansQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<GetVeterinarianResponse> veterinarians = await repository.GetResponsesAsync(request.SearchParameters, cancellationToken);
        List<GetVeterinarianResponse> responses = [.. veterinarians];

        return Result.Ok(responses);
    }
}
