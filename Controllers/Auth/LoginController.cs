using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using mini_blog.DTO.AuthDTO;
using mini_blog.DTO.UserDTO;
using mini_blog.Services;

namespace mini_blog.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(IAuthService authService, IAntiforgery antiforgery) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IAntiforgery _antiforgery = antiforgery;

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, IValidator<LoginDto> validator)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
            
            var user = await _authService.ValidateCredentialsAsync(dto.Email, dto.Password);
            if (user == null) return Unauthorized(new {message = "Invalid credentials"});

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("username", user.Username)
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Lax,
                Secure = true,
                IsEssential = true
            } );

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
            };
            
            return Ok(userDto);
        }
        
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return NoContent();
        }
    }
}
