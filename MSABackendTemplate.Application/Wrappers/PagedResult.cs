namespace MSABackendTemplate.Application.Wrappers;

/// <summary>
/// Generic wrapper for paginated results.
/// Contains items for current page along with pagination metadata.
/// </summary>
/// <typeparam name="T">Type of items in the collection</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Items for the current page
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Creates a paginated result
    /// </summary>
    public static PagedResult<T> Create(List<T> items, int count, int pageNumber, int pageSize)
    {
        return new PagedResult<T>
        {
            Items = items,
            TotalCount = count,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };
    }
}
