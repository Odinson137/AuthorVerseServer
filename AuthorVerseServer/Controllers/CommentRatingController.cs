using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial.Arenas;
using System.ComponentModel.Design;
using System.Xml.Linq;

namespace AuthorVerseServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentRatingController : ControllerBase
    {
        private readonly ICommentRating _rating;
        private readonly CreateJWTtokenService _jWTtokenService;
        public CommentRatingController(ICommentRating rating, CreateJWTtokenService jWTtokenService)
        {
            _rating = rating;
            _jWTtokenService = jWTtokenService;
        }

        [HttpPost("Up")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> ChangeUpRating(int commentId)
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            var rating = await _rating.GetRatingAsync(userId, commentId);

            if (rating == Data.Enums.LikeRating.NotRated)
            {
                var commentRating = new CommentRating
                {
                    CommentId = commentId,
                    Rating = Data.Enums.LikeRating.Like,
                    UserCommentedId = userId
                };
                await _rating.AddRatingAsync(commentRating);

                await _rating.SaveAsync();

                _rating.ChangeCountRating(commentId, 0, 1);
            }
            else if (rating == Data.Enums.LikeRating.DisLike)
            {
                await _rating.ChangeRatingAsync(commentId, Data.Enums.LikeRating.Like);
                _rating.ChangeCountRating(commentId, -1, 1);
            }
            else
            {
                return BadRequest("Already exist");
            }

            return Ok();
        }

        [HttpPost("Down")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> ChangeDownRating(int commentId)
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            var rating = await _rating.GetRatingAsync(userId, commentId);

            if (rating == Data.Enums.LikeRating.NotRated)
            {
                await _rating.AddRatingAsync(
                    new CommentRating
                    {
                        CommentId = commentId,
                        Rating = Data.Enums.LikeRating.DisLike,
                        UserCommentedId = userId
                    });

                await _rating.SaveAsync();
                _rating.ChangeCountRating(commentId, 1, 0);
            }
            else if (rating == Data.Enums.LikeRating.Like)
            {
                await _rating.ChangeRatingAsync(commentId, Data.Enums.LikeRating.DisLike);
                _rating.ChangeCountRating(commentId, 1, -1);
            }
            else
            {
                return BadRequest("Already exist");
            }

            return Ok();
        }

        [HttpDelete("Delete")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> DeleteRating(int commentId)
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            var rating = await _rating.GetRatingAsync(userId, commentId);

            if (rating == Data.Enums.LikeRating.NotRated)
            {
                return NotFound("Comment not found");
            }

            await _rating.DeleteRatingAsync(commentId);
            Change(commentId, rating);


            return Ok();
        }

        private void Change(int commentId, Data.Enums.LikeRating rating)
        {
            if (rating == Data.Enums.LikeRating.Like)
                _rating.ChangeCountRating(commentId, 0, -1);
            else if (rating == Data.Enums.LikeRating.DisLike)
                _rating.ChangeCountRating(commentId, -1, 0);
        }
    }
}
