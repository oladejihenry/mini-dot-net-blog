using FluentValidation;
using mini_blog.DTO.AuthDTO;

namespace mini_blog.Validation.Auth;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username).NotNull().NotEmpty().MaximumLength(100).WithMessage("Username is required");
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().WithMessage("Email is required");
        RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(8).WithMessage("Password is required");
    }
}