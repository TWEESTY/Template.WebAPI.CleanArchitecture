namespace ProjectName.Application.Common.Search;

/// <summary>
/// Specifies the logical operators for combining filter criteria.
/// </summary>
public enum LogicalOperator
{
    And,
    Or
}

/// <summary>
/// Specifies the direction for sorting.
/// </summary>
public enum OrderDirection
{
    Ascending,
    Descending
}

/// <summary>
/// Specifies the operators for filtering data.
/// </summary>
public enum FilterOperator
{
    Equal,
    NotEqual,
    IsNull,
    IsNotNull,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    StartsWith,
    DoesNotStartWith,
    EndsWith,
    DoesNotEndWith,
    Contains,
    DoesNotContain,
    IsEmpty,
    IsNotEmpty,
    Between
}

/// <summary>
/// Specifies the types of joins for combining tables.
/// </summary>
public enum JoinType
{
    Inner,
    Left,
    Right,
    Full,
    Self
}

/// <summary>
/// Represents a filter criterion with a name, operator, and value.
/// </summary>
/// <param name="Name">The name of the field to filter.</param>
/// <param name="FilterOperator">The operator to use for filtering.</param>
/// <param name="Value">The value to filter by.</param>
public record FilterCriteria(
        string Name,
        FilterOperator FilterOperator,
        string Value);

/// <summary>
/// Represents a nested filter group with its own logical operator and child items.
/// <summary>
/// Marker interface for items that can appear inside a <see cref="FilterCriteriaGroup"/>.
/// Implemented by <see cref="FilterCriteriaLeaf"/> and <see cref="FilterCriteriaGroup"/>.
/// </summary>
public interface IFilterItem;

/// <summary>
/// Represents a nested filter group with its own logical operator and child items.
/// Child items can be leaf <see cref="FilterCriteria"/> or nested <see cref="FilterCriteriaGroup"/>.
/// </summary>
/// <param name="LogicalLink">The logical operator (AND/OR) joining the children.</param>
/// <param name="Items">The child filter items (leaf criteria or nested groups).</param>
public record FilterCriteriaGroup(
        LogicalOperator LogicalLink,
        IEnumerable<IFilterItem> Items) : IFilterItem;

/// <summary>
/// A leaf filter item wrapping a single <see cref="FilterCriteria"/>.
/// </summary>
public record FilterCriteriaLeaf(FilterCriteria Criteria) : IFilterItem;

/// <summary>
/// Represents a filter criterion with a logical link and case sensitivity.
/// </summary>
/// <param name="FilterCriteria">The filter criteria.</param>
/// <param name="IsCaseSensitive">Indicates if the filter is case sensitive.</param>
/// <param name="LogicalOperator">The logical operator to link with other criteria.</param>
public record FilterCriteriaWithLogicalLink(FilterCriteria FilterCriteria,
    bool IsCaseSensitive = true,
    LogicalOperator LogicalOperator = LogicalOperator.And);

/// <summary>
/// Represents join criteria with a join type, table name, and filter criteria.
/// </summary>
/// <param name="JoinType">The type of join.</param>
/// <param name="TableName">The name of the table to join.</param>
/// <param name="FilterCriteriasWithLogicalLink">The filter criteria with logical links.</param>
public record JoinCriteria(JoinType JoinType, string TableName,
    IEnumerable<FilterCriteriaWithLogicalLink> FilterCriteriasWithLogicalLink);


/// <summary>
/// Represents the parameters for a search operation.
/// </summary>
/// <param name="FilterCriteriaParams">The filter criteria parameters.</param>
/// <param name="SortCriteriaCollection">The collection of sort criteria.</param>
/// <param name="GroupCriteriaCollection">The collection of group criteria.</param>
/// <param name="PageNumber">The page number for pagination.</param>
/// <param name="PageSize">The page size for pagination.</param>
public record SearchParameters(
    SearchParameters.FilterCriteriaCollection? FilterCriteriaParams = null,
    IEnumerable<SearchParameters.SortCriteria>? SortCriteriaCollection = null,
    IEnumerable<SearchParameters.GroupCriteria>? GroupCriteriaCollection = null,
    int? PageNumber = null,
    int? PageSize = null)
{
    public record FilterCriteriaCollection(
        LogicalOperator LogicalLink,
        IEnumerable<FilterCriteria> FilterParameters,
        IEnumerable<FilterCriteriaGroup>? NestedGroups = null);
    public record SortCriteria(string Field, OrderDirection Direction = OrderDirection.Ascending);
    public record GroupCriteria(string Field, OrderDirection Direction = OrderDirection.Ascending);
}