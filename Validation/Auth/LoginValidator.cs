using FluentValidation;
using mini_blog.DTO.AuthDTO;

namespace mini_blog.Validation.Auth;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().WithMessage("Email is required");
        
        RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password is required");
    }
}