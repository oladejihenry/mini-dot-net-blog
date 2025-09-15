namespace mini_blog.DTO.CommentDTO;

public class CreateCommentDto
{
    public string Content { get; set; } = null!;
    
    //Foreign Key
    public Guid PostId { get; set; }
}