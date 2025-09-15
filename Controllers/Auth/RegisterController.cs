using Microsoft.AspNetCore.Mvc;
using mini_blog.Entities;

namespace mini_blog.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<User>> Register(User user)
        {
            return Ok(user);
        }
    }
}
