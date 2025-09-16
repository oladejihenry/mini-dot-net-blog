using mini_blog.DTO.CommentDTO;
using mini_blog.Entities;

namespace mini_blog.DTO.PostDTO;

public class PostWithCommentsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;

    public string Slug { get; set; } = null!;

    //Navigtion Properties
    public object Author { get; set; } = null!;
    public List<GetCommentDto> Comments { get; set; } = new();
}