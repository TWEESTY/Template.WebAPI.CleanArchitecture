namespace ProjectName.Web.Api.Common.Search;

/// <summary>
/// Represents the search parameters used for filtering, sorting, grouping, and paginating data in the web API.
/// This record encapsulates the query parameters that can be passed in HTTP requests to customize the retrieval of data based on specific criteria. It provides methods to bind query parameters from the HTTP context and convert them into application-level search parameters for further processing.
/// </summary>
/// <param name="Page">The page number for pagination.</param>
/// <param name="PageSize">The number of items per page for pagination.</param>
/// <param name="Sort">The sorting criteria.</param>
/// <param name="Filter">The filtering criteria.</param>
/// <param name="Group">The grouping criteria.</param>
internal sealed record SearchParameters(int? Page = null, int? PageSize = null, string? Sort = null, string? Filter = null, string? Group = null)
{
    public static ValueTask<SearchParameters?> BindAsync(HttpContext context)
    {
        IQueryCollection query = context.Request.Query;
        int? page = int.TryParse(query["page"], out int p) ? p : null;
        int? pageSize = int.TryParse(query["pageSize"], out int ps) ? ps : null;
        string? sort = query["sort"];
        string? filter = query["filter"];
        string? group = query["group"];
        return ValueTask.FromResult<SearchParameters?>(new SearchParameters(page, pageSize, sort, filter, group));
    }

    public Application.Common.Search.SearchParameters ToApplicationSearchParameters()
    {
        return new Application.Common.Search.SearchParameters(
            SearchParametersHelper.GetFilterCriterias(Filter),
            SearchParametersHelper.GetSortCriterias(Sort),
            SearchParametersHelper.GetGroupCriterias(Group),
            Page,
            PageSize
        );
    }
}
