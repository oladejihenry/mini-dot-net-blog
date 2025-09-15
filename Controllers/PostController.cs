using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mini_blog.Data;
using mini_blog.DTO.CommentDTO;
using mini_blog.DTO.PostDTO;
using mini_blog.Entities;
using mini_blog.Helpers;

namespace mini_blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController(BlogDbContext context, SlugService slugService) : ControllerBase
    {
        private readonly BlogDbContext _context = context;
        private readonly SlugService _slugService = slugService;
        
        // GET: api/Post
        [HttpGet]
        public async Task<ActionResult<List<Post>>> GetPosts()
        {
            var posts = await _context.Posts.ToListAsync();
            return Ok(posts);
        }
        
        // POST: api/Post
        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost(CreatePostDto newPost, IValidator<CreatePostDto> validator)
        {
            var validationResult = await validator.ValidateAsync(newPost);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
            
            var slug = await _slugService.GenerateUniqueSlugFromTitle(newPost.Title);
            

            var post = new Post
            {
                Title = newPost.Title,
                Content = newPost.Content,
                Slug  = slug

            };
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPostById", new {id = post.Id}, post);
        }
        
        // GET: api/Post/guid
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPostById(Guid id)
        {
            var post = await _context.Posts.Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (post == null)
                return NotFound();
            
            var postDto = new PostWithCommentsDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Slug = post.Slug,
                Comments = post.Comments.Select(c => new GetCommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                }).ToList()
            };
                
            return Ok(postDto);
        }
        
        // PUT: api/Post/guid
        [HttpPut("{id}")]
        public async Task<ActionResult<Post>> UpdatePost(Guid id, UpdatePostDto updatePost, IValidator<UpdatePostDto> validator)
        {
            var validationResult = await validator.ValidateAsync(updatePost);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
            
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return NotFound();
            
            post.Title = updatePost.Title;
            post.Content = updatePost.Content;
            
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
        // DELETE: api/Post/guid
        [HttpDelete("{id}")]
        public async Task<ActionResult<Post>> DeletePost(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return NotFound();
            
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
                
            return NoContent();
        }
        
    }
}
