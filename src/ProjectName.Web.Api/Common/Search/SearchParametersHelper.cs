using System.Text.Json;
using ProjectName.Application.Common.Search;

namespace ProjectName.Web.Api.Common.Search;

/// <summary>
/// Provides helper methods for processing search parameters, including filtering, sorting, and grouping.
/// This class is designed to work with data structures commonly used in Kendo UI grids.
/// </summary>
public static class SearchParametersHelper
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static Application.Common.Search.SearchParameters.FilterCriterias? GetFilterCriterias(string? filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            return null;
        }

        FilterObject? filterObject;
        try
        {
            filterObject = JsonSerializer.Deserialize<FilterObject>(filter, _jsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }

        if (filterObject == null)
        {
            return null;
        }

        LogicalOperator logicalLink = ParseLogicalOperator(filterObject.Logic);

        List<FilterCriteria> leafFilters = [];
        List<FilterCriteriaGroup> nestedGroups = [];

        foreach (Filter f in filterObject.Filters)
        {
            if (IsNestedGroup(f))
            {
                nestedGroups.Add(ParseFilterGroup(f));
            }
            else
            {
                FilterCriteria? leaf = ParseLeafFilter(f);
                if (leaf != null)
                {
                    leafFilters.Add(leaf);
                }
            }
        }

        return new Application.Common.Search.SearchParameters.FilterCriterias(
            logicalLink,
            leafFilters,
            nestedGroups.Count > 0 ? nestedGroups : null);
    }

    private static FilterCriteriaGroup ParseFilterGroup(Filter filterObj)
    {
        LogicalOperator groupLogic = ParseLogicalOperator(filterObj.Logic);
        List<IFilterItem> items = [];

        foreach (Filter child in filterObj.Filters ?? [])
        {
            if (IsNestedGroup(child))
            {
                items.Add(ParseFilterGroup(child));
            }
            else
            {
                FilterCriteria? leaf = ParseLeafFilter(child);
                if (leaf != null)
                {
                    items.Add(new FilterCriteriaLeaf(leaf));
                }
            }
        }

        return new FilterCriteriaGroup(groupLogic, items);
    }

    private static bool IsNestedGroup(Filter f)
    {
        return f.Filters != null && f.Filters.Count > 0 && !string.IsNullOrEmpty(f.Logic);
    }

    private static FilterCriteria? ParseLeafFilter(Filter f)
    {
        if (string.IsNullOrEmpty(f.Operator) || !_operatorToFilterOperator.TryGetValue(f.Operator, out FilterOperator op))
        {
            return null;
        }

        if (op == FilterOperator.Between)
        {
            string[] values = (f.Value ?? string.Empty).Split(',');
            string value1 = values.Length > 0 ? values[0].Trim() : string.Empty;
            string value2 = values.Length > 1 ? values[1].Trim() : string.Empty;
            return new FilterCriteria(f.Field, FilterOperator.Between, $"{value1},{value2}");
        }

        return new FilterCriteria(f.Field, op, f.Value ?? string.Empty);
    }
    private static LogicalOperator ParseLogicalOperator(string? logic)
    {
        return string.Equals(logic, "and", StringComparison.OrdinalIgnoreCase) ? LogicalOperator.And : LogicalOperator.Or;
    }

    public static IEnumerable<Application.Common.Search.SearchParameters.SortCriteria> GetSortCriterias(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return [];
        }

        List<SortObject>? sortObjects;
        try
        {
            sortObjects = JsonSerializer.Deserialize<List<SortObject>>(sort, _jsonOptions);
        }
        catch (JsonException)
        {
            return [];
        }

        if (sortObjects == null || sortObjects.Count == 0)
        {
            return [];
        }

        return sortObjects.Select(so => new Application.Common.Search.SearchParameters.SortCriteria(
     so.Field,
     (so.Dir?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false) ? OrderDirection.Descending : OrderDirection.Ascending
 ));
    }


    public static IEnumerable<Application.Common.Search.SearchParameters.GroupCriteria> GetGroupCriterias(string? group)
    {
        if (string.IsNullOrEmpty(group))
        {
            return [];
        }

        group = group.Trim();
        if (group.StartsWith('[') || group.StartsWith('{'))
        {
            List<GroupObject>? groupObjects;
            try
            {
                groupObjects = JsonSerializer.Deserialize<List<GroupObject>>(group, _jsonOptions);
            }
            catch (JsonException)
            {
                return [];
            }

            if (groupObjects != null && groupObjects.Count > 0)
            {
                return groupObjects.Select(go => new Application.Common.Search.SearchParameters.GroupCriteria(
                    go.Field,
                    go.Dir != null && go.Dir.Equals("desc", StringComparison.OrdinalIgnoreCase)
                        ? OrderDirection.Descending
                        : OrderDirection.Ascending
                ));
            }

        }
        return [];
    }

    private sealed class FilterObject
    {
        public List<Filter> Filters { get; set; } = [];
        public string Logic { get; set; } = string.Empty;
    }

    private sealed class Filter
    {
        public string Field { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        // Nested composite group support
        public string? Logic { get; set; }
        public List<Filter>? Filters { get; set; }
    }

    private static readonly Dictionary<string, FilterOperator> _operatorToFilterOperator = new()
    {
        { "eq", FilterOperator.Equal },
        { "neq", FilterOperator.NotEqual },
        { "isnull", FilterOperator.IsNull },
        { "isnotnull", FilterOperator.IsNotNull },
        { "lt", FilterOperator.LessThan },
        { "lte", FilterOperator.LessThanOrEqual },
        { "gt", FilterOperator.GreaterThan },
        { "gte", FilterOperator.GreaterThanOrEqual },
        { "startswith", FilterOperator.StartsWith },
        { "doesnotstartwith", FilterOperator.DoesNotStartWith },
        { "endswith", FilterOperator.EndsWith },
        { "doesnotendwith", FilterOperator.DoesNotEndWith },
        { "contains", FilterOperator.Contains },
        { "doesnotcontain", FilterOperator.DoesNotContain },
        { "isempty", FilterOperator.IsEmpty },
        { "isnotempty", FilterOperator.IsNotEmpty },
        { "between", FilterOperator.Between },
    };
    private sealed class SortObject
    {
        public string Field { get; set; } = string.Empty;
        public string Dir { get; set; } = string.Empty;
    }
    private sealed class GroupObject
    {
        public string Field { get; set; } = string.Empty;
        public string? Dir { get; set; } = string.Empty;
    }
}
