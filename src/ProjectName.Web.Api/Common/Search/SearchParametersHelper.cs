using System.Text.Json;
using ProjectName.Application.Common.Search;

namespace ProjectName.Web.Api.Common.Search;

/// <summary>
/// Provides helper methods for processing search parameters, including filtering, sorting, and grouping.
/// This class is designed to work with data structures commonly used in Kendo UI grids.
/// </summary>
public static class SearchParametersHelper
{

    public static Application.Common.Search.SearchParameters.FilterCriteriaCollection? GetFilterCriterias(string? filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            return null;
        }

        FilterObject? filterObject = JsonSerializer.Deserialize<FilterObject>(filter);
        if (filterObject == null)
        {
            return null;
        }

        LogicalOperator logicalLink = ParseLogicalOperator(filterObject.Logic);

        var leafFilters = new List<FilterCriteria>();
        var nestedGroups = new List<FilterCriteriaGroup>();

        foreach (var f in filterObject.Filters)
        {
            if (IsNestedGroup(f))
            {
                nestedGroups.Add(ParseFilterGroup(f));
            }
            else
            {
                var leaf = ParseLeafFilter(f);
                if (leaf != null) leafFilters.Add(leaf);
            }
        }

        return new Application.Common.Search.SearchParameters.FilterCriteriaCollection(
            logicalLink,
            leafFilters,
            nestedGroups.Count > 0 ? nestedGroups : null);
    }

    private static FilterCriteriaGroup ParseFilterGroup(Filter filterObj)
    {
        LogicalOperator groupLogic = ParseLogicalOperator(filterObj.Logic);
        var items = new List<IFilterItem>();

        foreach (var child in filterObj.Filters ?? [])
        {
            if (IsNestedGroup(child))
            {
                items.Add(ParseFilterGroup(child));
            }
            else
            {
                var leaf = ParseLeafFilter(child);
                if (leaf != null) items.Add(new FilterCriteriaLeaf(leaf));
            }
        }

        return new FilterCriteriaGroup(groupLogic, items);
    }

    private static bool IsNestedGroup(Filter f) =>
        f.Filters != null && f.Filters.Count > 0 && !string.IsNullOrEmpty(f.Logic);

    private static FilterCriteria? ParseLeafFilter(Filter f)
    {
        if (string.IsNullOrEmpty(f.Operator) || !OperatorToFilterOperator.TryGetValue(f.Operator, out var op))
        {
            return null;
        }

        if (op == FilterOperator.Between)
        {
            var values = (f.Value ?? string.Empty).Split(',');
            var value1 = values.Length > 0 ? values[0].Trim() : string.Empty;
            var value2 = values.Length > 1 ? values[1].Trim() : string.Empty;
            return new FilterCriteria(f.Field, FilterOperator.Between, $"{value1},{value2}");
        }

        return new FilterCriteria(f.Field, op, f.Value ?? string.Empty);
    }
    private static LogicalOperator ParseLogicalOperator(string? logic) =>
        string.Equals(logic, "and", StringComparison.OrdinalIgnoreCase) ? LogicalOperator.And : LogicalOperator.Or;


    public static IEnumerable<Application.Common.Search.SearchParameters.SortCriteria> GetSortCriterias(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return Enumerable.Empty<Application.Common.Search.SearchParameters.SortCriteria>();
        }

        List<SortObject>? sortObjects = JsonSerializer.Deserialize<List<SortObject>>(sort);
        if (sortObjects == null || sortObjects.Count == 0)
        {
            return Enumerable.Empty<Application.Common.Search.SearchParameters.SortCriteria>();
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
            return Enumerable.Empty<Application.Common.Search.SearchParameters.GroupCriteria>();
        }

        group = group.Trim();
        if (group.StartsWith('[') || group.StartsWith('{'))
        {
            List<GroupObject>? groupObjects = JsonSerializer.Deserialize<List<GroupObject>>(group);
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
        return Enumerable.Empty<Application.Common.Search.SearchParameters.GroupCriteria>();
    }

    private sealed class FilterObject
    {
        public required List<Filter> Filters { get; set; } = new();
        public required string Logic { get; set; } = string.Empty;
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

    private static readonly Dictionary<string, FilterOperator> OperatorToFilterOperator = new()
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
        public required string Field { get; set; } = string.Empty;
        public required string Dir { get; set; } = string.Empty;
    }
    private sealed class GroupObject
    {
        public required string Field { get; set; } = string.Empty;
        public string? Dir { get; set; } = string.Empty;
    }
}
