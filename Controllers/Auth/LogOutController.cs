using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_blog.Services;

namespace mini_blog.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogOutController : ControllerBase
    {
        [HttpPost()]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return NoContent();
        }
    }
}
