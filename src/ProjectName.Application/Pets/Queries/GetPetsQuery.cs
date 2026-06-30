using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Queries;

/// <summary>
/// Represents a query to retrieve a list of pets based on optional search parameters in the application.
/// </summary>
/// <param name="SearchParameters">The search parameters to filter the pets.</param>
public sealed record GetPetsQuery(SearchParameters? SearchParameters) : IQuery<Result<List<GetPetResponse>>>, IHasSearchParameters;

public sealed class GetPetsQueryValidator : SearchParametersQueryValidator<GetPetsQuery>;

internal sealed class GetPetsHandler(IPetRepository petRepository) : IQueryHandler<GetPetsQuery, Result<List<GetPetResponse>>>
{
    async ValueTask<Result<List<GetPetResponse>>> IQueryHandler<GetPetsQuery, Result<List<GetPetResponse>>>.Handle(GetPetsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<GetPetResponse> pets = await petRepository.GetResponsesAsync(request.SearchParameters, cancellationToken);
        List<GetPetResponse> responses = [.. pets];

        return Result.Ok(responses);
    }
}
