using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IComment _comment;
        private readonly UserManager<User> _userManager;

        public CommentController(IComment comment, UserManager<User> userManager)
        {
            _comment = comment;
            _userManager = userManager;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<Comment>>> GetComment()
        {
            var comments = await _comment.GetCommentAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(comments);
        }

        [HttpPost("Create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<int>> CreateComment([FromBody] CreateCommentDTO commentDTO)
        {
            User? user = await _userManager.FindByIdAsync(commentDTO.UserId);
            if(user == null)
                return NotFound("User not found");

            Book? book = await _comment.GetBook(commentDTO.BookId);
            if (book == null)
                return NotFound("Book not found");

            if (_comment.CheckUserComment(book, user) != null)
                return BadRequest("This user alredy made a comment");



                Comment newComment = new Comment()
            {
                Commentator = user,
                BookId = commentDTO.BookId,
                Book = book,
                Text = commentDTO.Text,
            };

            await _comment.AddComment(newComment);
            return Ok();
        }

        [HttpPost("Delete")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteComment(int commentId, string userId)
        {
            bool result = await _comment.DeleteComment(commentId, userId);
            if (result == true)
                return Ok();
            else
                return BadRequest(new MessageDTO { message = "Коммаентарий не был удалён" });
        }
    }
}
