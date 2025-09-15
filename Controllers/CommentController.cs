using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mini_blog.Data;
using mini_blog.DTO.CommentDTO;
using mini_blog.Entities;

namespace mini_blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController(BlogDbContext context) : ControllerBase
    {
        private readonly BlogDbContext _context = context;
        
        // GET: api/Comment
        [HttpGet]
        public async Task<ActionResult<List<Comment>>> GetComments()
        {
            var comments = await  _context.Comments.ToListAsync();
            return Ok(comments);
        }
        
        //POST: api/Comment
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(CreateCommentDto newComment)
        {
            var comment = new Comment()
            {
                Content = newComment.Content,
                PostId = newComment.PostId
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCommentById", new {id = comment.Id}, comment);
        }
        
        //Get: api/Comment/guid
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetCommentById(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();
            return Ok(comment);
        }
        
        //DELETE: api/comment/guid
        [HttpDelete("{id}")]
        public async Task<ActionResult<Comment>> DeleteComment(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();
            
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
