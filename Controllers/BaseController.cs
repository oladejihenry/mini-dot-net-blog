namespace mini_blog.Controllers;
using Microsoft.AspNetCore.Mvc;
using mini_blog.DTO.Common;

[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult Success<T>(T data, string message = "Success")
    {
        return Ok(ApiResponse<T>.SuccessResponse(data, message));
    }

    protected IActionResult Error(string message, List<string>? errors = null)
    {
        return BadRequest(ApiResponse<object>.ErrorResponse(message, errors));
    }

    protected IActionResult Error(string message, string error)
    {
        return BadRequest(ApiResponse<object>.ErrorResponse(message, error));
    }

    protected IActionResult NotFound(string message = "Resource not found")
    {
        return NotFound(ApiResponse<object>.ErrorResponse(message));
    }

    protected IActionResult Unauthorized(string message = "Unauthorized")
    {
        return Unauthorized(ApiResponse<object>.ErrorResponse(message));
    }

    protected IActionResult Paginated<T>(T data, PaginationMeta meta, string message = "Success")
    {
        return Ok(new PaginatedResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Meta = meta
        });
    }
}