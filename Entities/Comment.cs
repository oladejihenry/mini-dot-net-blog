using System.ComponentModel.DataAnnotations;

namespace mini_blog.Entities;

public class Comment
{
    
    public Guid Id { get; set; }
    [Required]
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    //Foreign Key
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
    
}