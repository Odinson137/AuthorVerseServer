using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IComment _comment;
        public DataContext _context { get; set; }

        public CommentController(DataContext context, IComment comment)
        {
            _context = context;
            _comment = comment;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<Comment>>> GetComment()
        {
            var comments = await _comment.GetCommentAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(comments);
        }

        /*[HttpGet("{bookId}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<CommentDTO>>> GetBookComments(int bookId)
        {
            var comments = await _context.Comments.AsNoTracking()
                .Include(comment => comment.Commentator)
                .Where(comments => comments.BookId == bookId)
                .Select(comment => new CommentDTO()
                {
                    CommentId = comment.CommentId,
                    Commentator = new UserDTO { Id = comment.Commentator.Id, UserName = comment.Commentator.UserName }
                })
            .ToListAsync();

            return comments;
        }*/
    }
}
