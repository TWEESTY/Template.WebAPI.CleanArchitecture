using System.Reflection;

namespace ProjectName.Web.Api.Common.Search;

public record SearchParameters(int? Page = null, int? PageSize = null, string? Sort = null, string? Filter = null, string? Group = null)
{
    public static ValueTask<SearchParameters?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var query = context.Request.Query;
        int? page = int.TryParse(query["page"], out var p) ? p : null;
        int? pageSize = int.TryParse(query["pageSize"], out var ps) ? ps : null;
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
