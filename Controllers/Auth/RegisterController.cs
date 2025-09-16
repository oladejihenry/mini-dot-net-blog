using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using mini_blog.DTO.AuthDTO;
using mini_blog.DTO.UserDTO;
using mini_blog.Services;

namespace mini_blog.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        
        // POST: api/Register
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, IValidator<RegisterDto> validator)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
            
            var user = await _authService.RegisterAsync(dto);
            if (user == null) return Conflict(new {message = "Email already exists"});

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
            
            return CreatedAtAction(null, userDto);
        }
    }
}
