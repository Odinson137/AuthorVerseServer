using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IComment _comment;
        private readonly UserManager<User> _userManager;
        private readonly CreateJWTtokenService _jWTtokenService;

        public CommentController(IComment comment, UserManager<User> userManager, CreateJWTtokenService jWTtokenService)
        {
            _comment = comment;
            _userManager = userManager;
            _jWTtokenService = jWTtokenService;
        }

        // can be Authorize
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<CommentDTO>>> GetBookWithAuthorComments(int bookId, int page = 1)
        {
            if (--page < 0)
            {
                return BadRequest("Bad page");
            }

            string? userId = _jWTtokenService.GetIdFromToken(this.User);

            var comments = await _comment.GetCommentsByBookAsync(bookId, page, userId);
            return Ok(comments);
        }

        [Authorize]
        [HttpPost("Create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<int>> CreateComment([FromBody] CreateCommentDTO commentDTO)
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            User? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            int bookId = await _comment.ChechExistBookAsync(commentDTO.BookId);
            if (bookId == 0)
                return NotFound("Book not found");

            if (await _comment.CheckExistCommentAsync(bookId, userId) == true)
                return BadRequest("This user alredy made a comment");

            Comment newComment = new Comment()
            {
                UserId = userId,
                BookId = commentDTO.BookId,
                Text = commentDTO.Text,
                ReaderRating = commentDTO.Rating,
                //Discriminator = "Comment"
            };

            await _comment.AddComment(newComment);
            await _comment.SaveAsync();
            return Ok();
        }

        [Authorize]
        [HttpPost("Delete")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> DeleteComment(int commentId)
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            var isExist = await _comment.CheckExistCommentAsync(commentId, userId);
            if (isExist == false)
                return NotFound("Comment not found");

            await _comment.DeleteComment(commentId);
            await _comment.SaveAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("ChangeCommentText")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> ChangeComment(int commentId, string bookText)
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            Comment? comment = await _comment.GetCommentAsync(commentId);
            if (comment != null)
                comment.Text = bookText;
            else
                return NotFound("Comment from this user to this book not found");

            if (await _comment.SaveAsync() == 0)
            {
                return BadRequest("There are problems with save");
            }

            return Ok();
        }
    }
}
