using Microsoft.EntityFrameworkCore;
using MSABackendTemplate.Application.Parameters;
using MSABackendTemplate.Application.Wrappers;

namespace MSABackendTemplate.Application.Extensions;

/// <summary>
/// Extension methods for IQueryable to support pagination, filtering, and sorting
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies pagination to an IQueryable
    /// </summary>
    public static async Task<PagedResult<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        PaginationParameters parameters)
    {
        var count = await source.CountAsync();
        
        var items = await source
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return PagedResult<T>.Create(items, count, parameters.PageNumber, parameters.PageSize);
    }

    /// <summary>
    /// Applies dynamic sorting based on property name
    /// </summary>
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> source,
        string? sortBy,
        string sortOrder = "asc")
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return source;

        var propertyInfo = typeof(T).GetProperty(sortBy);
        if (propertyInfo == null)
            return source;

        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
        var property = System.Linq.Expressions.Expression.Property(parameter, propertyInfo);
        var lambda = System.Linq.Expressions.Expression.Lambda(property, parameter);

        var methodName = sortOrder.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
        
        var resultExpression = System.Linq.Expressions.Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(T), propertyInfo.PropertyType },
            source.Expression,
            System.Linq.Expressions.Expression.Quote(lambda));

        return source.Provider.CreateQuery<T>(resultExpression);
    }
}
