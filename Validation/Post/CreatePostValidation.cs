using FluentValidation;
using mini_blog.DTO.PostDTO;

namespace mini_blog.Validation.Post;

public class CreatePostValidation : AbstractValidator<CreatePostDto>
{
    public CreatePostValidation()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty().WithMessage("Title is required");
        RuleFor(x => x.Content).NotNull().NotEmpty().WithMessage("Content is required");

    }
}