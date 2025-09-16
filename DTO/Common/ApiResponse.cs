namespace mini_blog.DTO.Common;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
    public object? Meta { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Data = data,
            Message = message,
            Success = true,
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Errors = errors ?? new List<string>(),
            Message = message,
            Success = false,
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, string error)
    {
        return new ApiResponse<T>
        {
            Errors = new List<string> { error },
            Message = message,
            Success = false,
        };
    }
}