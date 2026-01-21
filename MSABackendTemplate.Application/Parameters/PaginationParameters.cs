namespace MSABackendTemplate.Application.Parameters;

/// <summary>
/// Parameters for pagination, filtering, and sorting
/// </summary>
public class PaginationParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Page number (1-based). Default: 1
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Default: 10, Max: 100
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Property name to sort by (e.g., "Name", "Price")
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort order: "asc" or "desc". Default: "asc"
    /// </summary>
    public string SortOrder { get; set; } = "asc";

    /// <summary>
    /// Search term for filtering (searches across multiple fields)
    /// </summary>
    public string? SearchTerm { get; set; }
}
