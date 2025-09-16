namespace mini_blog.DTO.Common;

public class PaginatedResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "success";
    public T? Data { get; set; }
    public PaginationMeta Meta { get; set; } = new();
}

public class PaginationMeta
{
    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
    public int LastPage { get; set; }
    public int From { get; set; }
    public int To { get; set; }
    public bool HasMorePages { get; set; }
    public string? NextPageUrl { get; set; }
    public string? PrevPageUrl { get; set; }
}