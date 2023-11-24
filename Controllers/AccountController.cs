using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController :  ControllerBase
    {
        private readonly IAccount _account;
        private readonly CreateJWTtokenService _jWTtokenService;
        public AccountController(
            IAccount account, CreateJWTtokenService jWTtokenService)
        {
            _account = account;
            _jWTtokenService = jWTtokenService;
        }

        [HttpGet("Profile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile()
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            UserProfileDTO profile = await _account.GetUserAsync(userId);
            if (profile == null)
                return NotFound("User's not found");

            return Ok(profile);
        }

        [HttpGet("SelectedBooks")]
        public async Task<ActionResult<ICollection<SelectedUserBookDTO>>> GetSelectedBooks()
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            return Ok(await _account.GetUserSelectedBooksAsync(userId));
        }

        [HttpGet("UserComments")]
        public async Task<ActionResult<CommentPageDTO>> GetUserComments(
            CommentType commentType = CommentType.All, int page = 1, string searchComment = "") // показывать на одной странице по 10 комментов
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            if (--page < 0)
                return BadRequest("Page is smaller than zero");

            var comments = await _account.GetCommentsPagesCount(commentType, searchComment, userId);

            return Ok(comments);
/*            return Ok(new CommentPageDTO());
*/        }

        [HttpGet("Friends")]
        public async Task<ActionResult<ICollection<FriendDTO>>> GetFriends()
        {
            // userId from token
            return Ok(new List<FriendDTO>());
        }

        [HttpGet("UserBooks")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<UserBookDTO>>> GetUserBooks()
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            var books = await _account.GetUserBooksAsync(userId);
            return Ok(books);
        }

        [HttpGet("Updates")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<UpdateAccountBook>>> GetUserBooksUpdates()
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            var books = await _account.CheckUserUpdatesAsync(userId);

            return Ok(books);
        }
    }
}
