namespace UserApp.Web.ViewModels;

public class ListViewModel<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}