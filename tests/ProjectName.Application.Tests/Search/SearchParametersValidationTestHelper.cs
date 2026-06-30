using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Common.Search;

namespace ProjectName.Application.Tests.Search;

internal static class SearchParametersValidationTestHelper
{
    internal const string PageNumberIdentifier = "SearchParameters.PageNumber";
    internal const string PageSizeIdentifier = "SearchParameters.PageSize";

    internal static SearchParameters BuildInvalidPageNumber() => new(PageNumber: 0, PageSize: 10);

    internal static SearchParameters BuildInvalidPageSize() => new(PageNumber: 1, PageSize: 101);

    internal static async Task AssertInvalidPageNumberAsync<TResponse>(
        IMediator mediator,
        Func<SearchParameters, IQuery<Result<List<TResponse>>>> queryFactory)
    {
        Result<List<TResponse>> result = await mediator.Send(queryFactory(BuildInvalidPageNumber()));
        AssertValidationFailure(result, PageNumberIdentifier);
    }

    internal static async Task AssertInvalidPageSizeAsync<TResponse>(
        IMediator mediator,
        Func<SearchParameters, IQuery<Result<List<TResponse>>>> queryFactory)
    {
        Result<List<TResponse>> result = await mediator.Send(queryFactory(BuildInvalidPageSize()));
        AssertValidationFailure(result, PageSizeIdentifier);
    }

    internal static void AssertValidationFailure(ResultBase result, string expectedIdentifier)
    {
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error =>
            error is ValidationError validationError && validationError.Identifier == expectedIdentifier);
    }
}
