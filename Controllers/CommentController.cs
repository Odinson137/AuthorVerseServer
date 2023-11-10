﻿using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
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

        public CommentController(IComment comment, UserManager<User> userManager)
        {
            _comment = comment;
            _userManager = userManager;
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
            ClaimsPrincipal user = this.User;
            string? userId = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            return BadRequest(new MessageDTO { message = "Коммаентарий не был удалён" });
            //bool result = await _comment.DeleteComment(commentId, userId); // обновить DeleteComment
            //if (result == true)
            //    return Ok();
            //else
            //    return BadRequest(new MessageDTO { message = "Коммаентарий не был удалён" });
        }

        [Authorize]
        [HttpPost("ChangeCommentText")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> ChangeComment(int commentId, string bookText)
        {
            return BadRequest();
        }

        [Authorize]
        [HttpPost("UpRating")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> ChangeUpRating(int commentId)
        {
            return BadRequest();
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
