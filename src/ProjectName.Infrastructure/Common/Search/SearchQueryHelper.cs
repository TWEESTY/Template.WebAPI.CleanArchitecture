using System.Linq.Expressions;
using System.Reflection;
using ProjectName.Application.Common.Search;

namespace ProjectName.Infrastructure.Common.Search;

/// <summary>
/// Provides helper methods for applying search queries, including filtering, sorting, and pagination, to an IQueryable source based on specified search parameters and field selectors.    
/// </summary>
public static class SearchQueryHelper
{
    public static IQueryable<T> Apply<T>(
        IQueryable<T> source,
        SearchParameters? searchParameters,
        IReadOnlyDictionary<string, LambdaExpression> fieldSelectors,
        string? defaultSortField = null)
    {
        IQueryable<T> query = source;

        if (searchParameters?.FilterCriteriaParams is not null)
        {
            Expression<Func<T, bool>>? predicate = BuildFilterCollectionExpression<T>(searchParameters.FilterCriteriaParams, fieldSelectors);
            if (predicate is not null)
            {
                query = query.Where(predicate);
            }
        }

        query = ApplyOrdering(query, searchParameters?.GroupCriteriaCollection, searchParameters?.SortCriteriaCollection, fieldSelectors, defaultSortField);

        if (searchParameters?.PageNumber is int pageNumber &&
            searchParameters.PageSize is int pageSize &&
            pageNumber > 0 &&
            pageSize > 0)
        {
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        return query;
    }

    private static IQueryable<T> ApplyOrdering<T>(
        IQueryable<T> source,
        IEnumerable<SearchParameters.GroupCriteria>? groupCriteria,
        IEnumerable<SearchParameters.SortCriteria>? sortCriteria,
        IReadOnlyDictionary<string, LambdaExpression> fieldSelectors,
        string? defaultSortField)
    {
        IOrderedQueryable<T>? ordered = null;

        foreach (SearchParameters.GroupCriteria group in groupCriteria ?? [])
        {
            ordered = ApplyOrder(ordered, source, group.Field, group.Direction, fieldSelectors);
            if (ordered is not null)
            {
                source = ordered;
            }
        }

        foreach (SearchParameters.SortCriteria sort in sortCriteria ?? [])
        {
            ordered = ApplyOrder(ordered, source, sort.Field, sort.Direction, fieldSelectors);
            if (ordered is not null)
            {
                source = ordered;
            }
        }

        if (ordered is null &&
            !string.IsNullOrWhiteSpace(defaultSortField) &&
            TryGetFieldSelector(defaultSortField, fieldSelectors, out LambdaExpression? defaultSelector))
        {
            return ApplyOrderBy(source, defaultSelector!, isDescending: false);
        }

        return ordered ?? source;
    }

    private static IOrderedQueryable<T>? ApplyOrder<T>(
        IOrderedQueryable<T>? ordered,
        IQueryable<T> source,
        string field,
        OrderDirection direction,
        IReadOnlyDictionary<string, LambdaExpression> fieldSelectors)
    {
        if (!TryGetFieldSelector(field, fieldSelectors, out LambdaExpression? selector))
        {
            return ordered;
        }

        if (ordered is null)
        {
            return direction == OrderDirection.Descending
                ? ApplyOrderBy(source, selector!, isDescending: true)
                : ApplyOrderBy(source, selector!, isDescending: false);
        }

        return direction == OrderDirection.Descending
            ? ApplyThenBy(ordered, selector!, isDescending: true)
            : ApplyThenBy(ordered, selector!, isDescending: false);
    }

    private static Expression<Func<T, bool>>? BuildFilterCollectionExpression<T>(
        SearchParameters.FilterCriterias collection,
        IReadOnlyDictionary<string, LambdaExpression> fieldSelectors)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        List<Expression> nodes = [];

        foreach (FilterCriteria filter in collection.FilterParameters)
        {
            Expression? node = BuildFilterNode<T>(parameter, filter, fieldSelectors);
            if (node is not null)
            {
                nodes.Add(node);
            }
        }

        if (collection.NestedGroups is not null)
        {
            foreach (FilterCriteriaGroup group in collection.NestedGroups)
            {
                Expression? node = BuildFilterGroupNode<T>(parameter, group, fieldSelectors);
                if (node is not null)
                {
                    nodes.Add(node);
                }
            }
        }

        if (nodes.Count == 0)
        {
            return null;
        }

        Expression combined = CombineNodes(nodes, collection.LogicalLink);
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    private static Expression? BuildFilterGroupNode<T>(
        ParameterExpression parameter,
        FilterCriteriaGroup group,
        IReadOnlyDictionary<string, LambdaExpression> fieldSelectors)
    {
        List<Expression> nodes = [];

        foreach (IFilterItem filterItem in group.Items)
        {
            switch (filterItem)
            {
                case FilterCriteriaLeaf leaf:
                    {
                        Expression? node = BuildFilterNode<T>(parameter, leaf.Criteria, fieldSelectors);
                        if (node is not null)
                        {
                            nodes.Add(node);
                        }
                    }
                    break;
                case FilterCriteriaGroup nestedGroup:
                    {
                        Expression? node = BuildFilterGroupNode<T>(parameter, nestedGroup, fieldSelectors);
                        if (node is not null)
                        {
                            nodes.Add(node);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        if (nodes.Count == 0)
        {
            return null;
        }

        return CombineNodes(nodes, group.LogicalLink);
    }

    private static Expression? BuildFilterNode<T>(
        ParameterExpression parameter,
        FilterCriteria filter,
        IReadOnlyDictionary<string, LambdaExpression> fieldSelectors)
    {
        if (!TryGetFieldSelector(filter.Name, fieldSelectors, out LambdaExpression? selector))
        {
            return null;
        }

        Expression member = ReplaceParameter(selector!.Body, selector.Parameters[0], parameter);
        Type memberType = member.Type;
        Type underlyingType = Nullable.GetUnderlyingType(memberType) ?? memberType;

        return filter.FilterOperator switch
        {
            FilterOperator.IsNull => Expression.Equal(member, Expression.Constant(null, memberType)),
            FilterOperator.IsNotNull => Expression.NotEqual(member, Expression.Constant(null, memberType)),
            FilterOperator.IsEmpty => BuildIsEmpty(member),
            FilterOperator.IsNotEmpty => Expression.Not(BuildIsEmpty(member)),
            FilterOperator.Contains => BuildStringMethod(member, nameof(string.Contains), filter.Value),
            FilterOperator.DoesNotContain => Expression.Not(BuildStringMethod(member, nameof(string.Contains), filter.Value)),
            FilterOperator.StartsWith => BuildStringMethod(member, nameof(string.StartsWith), filter.Value),
            FilterOperator.DoesNotStartWith => Expression.Not(BuildStringMethod(member, nameof(string.StartsWith), filter.Value)),
            FilterOperator.EndsWith => BuildStringMethod(member, nameof(string.EndsWith), filter.Value),
            FilterOperator.DoesNotEndWith => Expression.Not(BuildStringMethod(member, nameof(string.EndsWith), filter.Value)),
            FilterOperator.Equal => BuildComparison(member, underlyingType, filter.Value, ExpressionType.Equal),
            FilterOperator.NotEqual => BuildComparison(member, underlyingType, filter.Value, ExpressionType.NotEqual),
            FilterOperator.LessThan => BuildComparison(member, underlyingType, filter.Value, ExpressionType.LessThan),
            FilterOperator.LessThanOrEqual => BuildComparison(member, underlyingType, filter.Value, ExpressionType.LessThanOrEqual),
            FilterOperator.GreaterThan => BuildComparison(member, underlyingType, filter.Value, ExpressionType.GreaterThan),
            FilterOperator.GreaterThanOrEqual => BuildComparison(member, underlyingType, filter.Value, ExpressionType.GreaterThanOrEqual),
            FilterOperator.Between => BuildBetween(member, underlyingType, filter.Value),
            _ => null
        };
    }

    private static bool TryGetFieldSelector(
        string field,
        IReadOnlyDictionary<string, LambdaExpression> fieldSelectors,
        out LambdaExpression? selector)
    {
        string normalized = field.Trim();
        return fieldSelectors.TryGetValue(normalized, out selector);
    }

    private static Expression CombineNodes(List<Expression> nodes, LogicalOperator logicalOperator)
    {
        Expression combined = nodes[0];
        for (int i = 1; i < nodes.Count; i++)
        {
            combined = logicalOperator == LogicalOperator.And
                ? Expression.AndAlso(combined, nodes[i])
                : Expression.OrElse(combined, nodes[i]);
        }

        return combined;
    }

    private static Expression BuildIsEmpty(Expression member)
    {
        if (member.Type != typeof(string))
        {
            return Expression.Constant(false);
        }

        Expression isNull = Expression.Equal(member, Expression.Constant(null, typeof(string)));
        Expression isEmpty = Expression.Equal(member, Expression.Constant(string.Empty));
        return Expression.OrElse(isNull, isEmpty);
    }

    private static Expression BuildStringMethod(Expression member, string methodName, string value)
    {
        if (member.Type != typeof(string))
        {
            return Expression.Constant(false);
        }

        MethodInfo method = typeof(string).GetMethod(methodName, [typeof(string)])!;
        Expression notNull = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
        Expression call = Expression.Call(member, method, Expression.Constant(value));
        return Expression.AndAlso(notNull, call);
    }

    private static Expression? BuildComparison(Expression member, Type underlyingType, string value, ExpressionType comparisonType)
    {
        object? convertedValue = ConvertToType(value, underlyingType);
        if (convertedValue is null)
        {
            return null;
        }

        Expression right = Expression.Constant(convertedValue, underlyingType);
        Expression left = member.Type == underlyingType ? member : Expression.Convert(member, underlyingType);

        if (member.Type != underlyingType)
        {
            Expression hasValue = Expression.Property(member, "HasValue");
            Expression valueAccess = Expression.Property(member, "Value");
            Expression comparison = Expression.MakeBinary(comparisonType, valueAccess, right);
            return comparisonType == ExpressionType.NotEqual
                ? Expression.OrElse(Expression.Not(hasValue), comparison)
                : Expression.AndAlso(hasValue, comparison);
        }

        return Expression.MakeBinary(comparisonType, left, right);
    }

    private static Expression? BuildBetween(Expression member, Type underlyingType, string expectedRange)
    {
        string[] parts = expectedRange.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return null;
        }

        Expression? lower = BuildComparison(member, underlyingType, parts[0], ExpressionType.GreaterThanOrEqual);
        Expression? upper = BuildComparison(member, underlyingType, parts[1], ExpressionType.LessThanOrEqual);

        if (lower is null || upper is null)
        {
            return null;
        }

        return Expression.AndAlso(lower, upper);
    }

    private static IOrderedQueryable<T> ApplyOrderBy<T>(IQueryable<T> source, LambdaExpression selector, bool isDescending)
    {
        string methodName = isDescending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

        MethodInfo method = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), selector.ReturnType);

        return (IOrderedQueryable<T>)method.Invoke(null, [source, selector])!;
    }

    private static IOrderedQueryable<T> ApplyThenBy<T>(IOrderedQueryable<T> source, LambdaExpression selector, bool isDescending)
    {
        string methodName = isDescending ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy);

        MethodInfo method = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), selector.ReturnType);

        return (IOrderedQueryable<T>)method.Invoke(null, [source, selector])!;
    }

    private static Expression ReplaceParameter(Expression body, ParameterExpression source, ParameterExpression target)
    {
        return new ParameterReplaceVisitor(source, target).Visit(body);
    }

    private sealed class ParameterReplaceVisitor(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == source ? target : base.VisitParameter(node);
        }
    }

    private static object? ConvertToType(string value, Type targetType)
    {
        if (targetType == typeof(Guid) && Guid.TryParse(value, out Guid guidValue))
        {
            return guidValue;
        }

        if (targetType == typeof(DateTimeOffset) && DateTimeOffset.TryParse(value, out DateTimeOffset dateTimeOffsetValue))
        {
            return dateTimeOffsetValue;
        }

        if (targetType == typeof(DateTime) && DateTime.TryParse(value, out DateTime dateTimeValue))
        {
            return dateTimeValue;
        }

        if (targetType == typeof(int) && int.TryParse(value, out int intValue))
        {
            return intValue;
        }

        if (targetType == typeof(long) && long.TryParse(value, out long longValue))
        {
            return longValue;
        }

        if (targetType == typeof(bool) && bool.TryParse(value, out bool boolValue))
        {
            return boolValue;
        }

        if (targetType == typeof(string))
        {
            return value;
        }

        // Support smart-enum/domain scalar patterns with static factory methods.
        MethodInfo? fromValueInt = targetType.GetMethod(
            "FromValue",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
            binder: null,
            types: [typeof(int)],
            modifiers: null);
        if (fromValueInt is not null && int.TryParse(value, out int intFromValue))
        {
            try
            {
                return fromValueInt.Invoke(null, [intFromValue]);
            }
            catch
            {
                return null;
            }
        }

        MethodInfo? tryFromValueInt = targetType.GetMethod(
            "TryFromValue",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
            binder: null,
            types: [typeof(int), targetType.MakeByRefType()],
            modifiers: null);
        if (tryFromValueInt is not null && int.TryParse(value, out int intTryFromValue))
        {
            object?[] args = [intTryFromValue, null];
            try
            {
                bool success = (bool)(tryFromValueInt.Invoke(null, args) ?? false);
                if (success)
                {
                    return args[1];
                }
            }
            catch
            {
                return null;
            }
        }

        MethodInfo? fromName = targetType.GetMethod(
            "FromName",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
            binder: null,
            types: [typeof(string), typeof(bool)],
            modifiers: null);
        if (fromName is not null)
        {
            try
            {
                return fromName.Invoke(null, [value, true]);
            }
            catch
            {
                return null;
            }
        }

        return null;
    }
}
