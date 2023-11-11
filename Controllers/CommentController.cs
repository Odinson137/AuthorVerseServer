using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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


        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Collection<CommentDTO>>> GetBookComment(int bookId)
        {
            return BadRequest();
        }

        [Authorize]
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

            if (await _comment.CheckUserComment(book, user) != null)
                return BadRequest("This user alredy made a comment");

            Comment newComment = new Comment()
            {
                Commentator = user,
                BookId = commentDTO.BookId,
                Book = book,
                Text = commentDTO.Text,
            };

            await _comment.AddComment(newComment);
            await _comment.Save();
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

            Comment? comment = await _comment.GetCommentAsync(commentId);
            if (comment == null)
                return NotFound("Comment not found");

            await _comment.DeleteComment(comment);
            if (await _comment.Save() == 0)
                return BadRequest("Problem with saving");

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

            Comment comment = await _comment.GetCommentAsync(commentId);
            if (comment != null)
                comment.Text = bookText;
            else
                return NotFound("Comment from this user to this book not found");

            return Ok();
        }

        [Authorize]
        [HttpPost("UpRating")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> ChangeUpRating(int commentId)
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            Comment? comment = await _comment.GetCommentAsync(commentId);
            if (comment != null)
                comment.commentRatings.Add(new CommentRating { CommentId = commentId, Likes = 1, UserCommentedId = userId });
            else
                return NotFound("Comment from this user to this book not found");

            if (await _comment.Save() == 0)
                return BadRequest("Problem with saving");

            return Ok();
        }

        [Authorize]
        [HttpPost("DownRating")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> ChangeDownRating(int commentId)
        {
            return BadRequest();
        }
    }
}
