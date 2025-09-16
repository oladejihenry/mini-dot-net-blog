using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mini_blog.Data;
using mini_blog.DTO.CommentDTO;
using mini_blog.DTO.Common;
using mini_blog.DTO.PostDTO;
using mini_blog.DTO.UserDTO;
using mini_blog.Entities;
using mini_blog.Helpers;

namespace mini_blog.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class PostController(BlogDbContext context, SlugService slugService) : BaseController
    {
        private readonly BlogDbContext _context = context;
        private readonly SlugService _slugService = slugService;

        // GET: api/Post - Get current user posts
        [HttpGet]
        public async Task<IActionResult> GetMyPosts([FromQuery] int page = 1, [FromQuery] int perPage = 1)
        {
            var user = HttpContext.CurrentUser();
            if (user is null) return Unauthorized(new { message = "Unauthorized" });

            var total = await _context.Posts.CountAsync(p => p.UserId == user.Id);
            var posts = await _context.Posts
                .Include(p => p.User)
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .ToListAsync();

            var getPosts = posts.Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                Slug = p.Slug,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Author = new UserDto
                {
                    Id = p.User.Id,
                    Username = p.User.Username,
                    Email = p.User.Email
                },
            }).ToList();

            var meta = new PaginationMeta
            {
                CurrentPage = page,
                PerPage = perPage,
                Total = total,
                LastPage = (int)Math.Ceiling((double)total / perPage),
                From = (page - 1) * perPage + 1,
                To = Math.Min(page * perPage, total),
                HasMorePages = page * perPage < total
            };

            return Paginated(getPosts, meta, "Posts retrieved successfully");

        }

        // POST: api/Post - Create post for current user
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostDto newPost, IValidator<CreatePostDto> validator)
        {
            var user = HttpContext.CurrentUser();
            if (user is null) return Unauthorized(new { message = "Unauthorized" });

            var validationResult = await validator.ValidateAsync(newPost);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var slug = await _slugService.GenerateUniqueSlugFromTitle(newPost.Title);


            var post = new Post
            {
                Title = newPost.Title,
                Content = newPost.Content,
                Slug = slug,
                UserId = user.Id

            };
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            var postDto = new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Slug = post.Slug,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Author = new UserDto
                {
                    Id = post.User.Id,
                    Username = post.User.Username,
                    Email = post.User.Email
                },
            };

            return Success(postDto, "Post created successfully");
        }

        // GET: api/Post/guid
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var user = HttpContext.CurrentUser();
            if (user is null) return Unauthorized(new { message = "Unauthorized" });

            var post = await _context.Posts
                .Include(p => p.Comments)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id);

            if (post == null)
                return NotFound("Post not found");

            var postDto = new PostWithCommentsDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Slug = post.Slug,
                Author = new UserDto
                {
                    Id = post.User.Id,
                    Username = post.User.Username,
                    Email = post.User.Email
                },
                Comments = post.Comments.Select(c => new GetCommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                }).ToList()
            };

            return Success(postDto, "Post retrieved successfully");
        }

        // PUT: api/Post/guid
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(Guid id, UpdatePostDto updatePost, IValidator<UpdatePostDto> validator)
        {
            var user = HttpContext.CurrentUser();
            if (user is null) return Unauthorized(new { message = "Unauthorized" });

            var validationResult = await validator.ValidateAsync(updatePost);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id);

            if (post == null)
                return NotFound("Post not found");

            post.Title = updatePost.Title;
            post.Content = updatePost.Content;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var postDto = new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Slug = post.Slug,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Author = new UserDto
                {
                    Id = post.User.Id,
                    Username = post.User.Username,
                    Email = post.User.Email
                },
            };

            return Success(postDto, "Post updated successfully");
        }

        // DELETE: api/Post/guid
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var user = HttpContext.CurrentUser();
            if (user is null) return Unauthorized(new { message = "Unauthorized" });

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id);
            if (post == null)
                return NotFound("Post not found");

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Success<object>("Post deleted successfully");
        }

    }
}
