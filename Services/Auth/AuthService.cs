using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mini_blog.Data;
using mini_blog.DTO.AuthDTO;
using mini_blog.Entities;

namespace mini_blog.Services.Auth;

public class AuthService : IAuthService
{
    private readonly BlogDbContext _db;
    private readonly IPasswordHasher<User> _hasher;
    
    public AuthService(BlogDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<User?> RegisterAsync(RegisterDto registerDto)
    {
        if(await _db.Users.AnyAsync(u => u.Email == registerDto.Email)) return null;

        var user = new User()
        {
            Email = registerDto.Email,
            Username = registerDto.Username
        };
        
        user.passwordHash = _hasher.HashPassword(user, registerDto.Password);
        
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        
        return user;
    }

    public async Task<User?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if(user == null) return null;
        
        var result = _hasher.VerifyHashedPassword(user, user.passwordHash, password);
        if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.passwordHash = _hasher.HashPassword(user, password);
                await _db.SaveChangesAsync();
            }
            
            return user;
        }
        
        return null;
    }
}