using FluentValidation;

namespace ProjectName.Application.Common.Search;

/// <summary>
/// Marker contract for queries that expose shared search parameters.
/// </summary>
public interface IHasSearchParameters
{
    SearchParameters? SearchParameters { get; }
}

/// <summary>
/// Shared paging validator for all queries that use <see cref="SearchParameters"/>.
/// </summary>
/// <typeparam name="TQuery">Query type exposing SearchParameters.</typeparam>
public abstract class SearchParametersQueryValidator<TQuery> : AbstractValidator<TQuery>
    where TQuery : IHasSearchParameters
{
    protected SearchParametersQueryValidator()
    {
        _ = RuleFor(x => x.SearchParameters!.PageNumber)
            .GreaterThan(0)
            .When(x => x.SearchParameters?.PageNumber is not null);

        _ = RuleFor(x => x.SearchParameters!.PageSize)
            .InclusiveBetween(1, 100)
            .When(x => x.SearchParameters?.PageSize is not null);
    }
}
