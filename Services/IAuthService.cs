using mini_blog.DTO.AuthDTO;
using mini_blog.Entities;

namespace mini_blog.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterDto registerDto);
    Task<User?> ValidateCredentialsAsync(string email,  string password);
}