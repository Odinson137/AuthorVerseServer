using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        public DataContext _context { get; set; }

        public CommentController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{bookId}")]
        [ProducesResponseType(200, Type = typeof(ICollection<CommentDTO>))]
        public async Task<IActionResult> GetBookComments(int bookId)
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

            return Ok(comments);
        }
    }
}
