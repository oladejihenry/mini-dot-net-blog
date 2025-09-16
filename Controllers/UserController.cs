using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mini_blog.Helpers;

namespace mini_blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetMe()
        {
            var user = HttpContext.CurrentUser();
            if(user is null) return Unauthorized(new {message = "Unauthorized"});
            return Ok(new {user.Id, user.Email, user.Username});
        }
    }
}
