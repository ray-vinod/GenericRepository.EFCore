namespace GenericRepository;

public class PagedList<TEntity>
{
    public IEnumerable<TEntity> Items { get; set; } = [];
    public int TotalItemCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalItemCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedList() { }

    public PagedList(IEnumerable<TEntity> items, int totalItemCount, int pageSize, int pageNumber)
    {
        Items = items; // paged items for the current page
        TotalItemCount = totalItemCount; // total number of items
        PageSize = pageSize;
        PageNumber = pageNumber;
    }
}