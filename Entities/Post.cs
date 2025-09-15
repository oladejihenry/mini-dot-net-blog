using System.ComponentModel.DataAnnotations;
using Slugify;

namespace mini_blog.Entities;

public class Post
{
    public Guid Id { get; set; }
    [Required] [StringLength(100)] public string Title { get; set; } = null!;
    [Required]
    public string Content { get; set; } = null!;
    [Required]
    public string Slug { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    //Navigtion Properties
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}