namespace NBlog.Api.Posts;

public class PagingMetadata
{
    const int MaxPageSize = 50;
    private int _pageSize = 10;
    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string OrderBy { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;

    public int Offset
    {
        get
        {
            return (PageNumber - 1) * PageSize;
        }
        private set { }
    }
}