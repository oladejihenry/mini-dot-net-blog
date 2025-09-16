using mini_blog.Entities;

namespace mini_blog.Helpers;

public static class HttpContextExtensions
{
        public static User? CurrentUser(this HttpContext context) => context.Items["CurrentUser"] as User;
}