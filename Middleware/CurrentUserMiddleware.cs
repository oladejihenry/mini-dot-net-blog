using System.Security.Claims;
using mini_blog.Data;

namespace mini_blog.Middleware;

public class CurrentUserMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, BlogDbContext db)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var id = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(id, out var userId))
            {
                var user = await db.Users.FindAsync(userId);
                context.Items["CurrentUser"] = user;
            }
        }
        
        await _next(context);
        
    }
    
    
}