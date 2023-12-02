using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;
        private readonly ILoadImage _loadImage;

        public AccountController(
            IAccount account, CreateJWTtokenService jWTtokenService, UserManager<User> userManager, ILoadImage loadImage)
        {
            _account = account;
            _jWTtokenService = jWTtokenService;
            _userManager = userManager;
            _loadImage = loadImage;
        }

        [HttpGet("Profile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
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
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
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
            
            int commentsCount = page == 0 ? await _account.GetCommentsPagesCount(commentType, searchComment, userId) : 0;

            var commentsFounded = await _account.GetUserCommentsAsync(commentType, page, searchComment, userId);

            return Ok(new CommentPageDTO//comments List а не ICollection
            {
                PagesCount = commentsCount,
                comments = commentsFounded
            });
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
        [ProducesResponseType(400, Type = typeof(string))]
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
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult<ICollection<UpdateAccountBook>>> GetUserBooksUpdates()
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            var books = await _account.CheckUserUpdatesAsync(userId);

            return Ok(books);
        }

        [HttpPut("ProfileChange")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorMessageDTO))]
        [ProducesResponseType(404, Type = typeof(ErrorMessageDTO))]
        public async Task<ActionResult> ChangeUserProfile(EditProfileDTO profile)
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ErrorMessageDTO("User not found"));
            }

            var passwordCheck = await _userManager.ChangePasswordAsync(user, profile.CheckPassword, profile.Password);
            if (passwordCheck.Succeeded == false)
            {
                return BadRequest(new ErrorMessageDTO(string.Join("; ", passwordCheck.Errors.Select(x => x.Description ))));
            }

            user.Name = profile.Name;
            user.LastName = profile.LastName;
            user.Description = profile.Description;
            if (profile.Logo != null)
            {
                string nameFile = _loadImage.GetUniqueName(profile.Logo);
                await _loadImage.CreateImageAsync(profile.Logo, nameFile, "Images");
                user.LogoUrl = nameFile;
            } else
            {
                if (!string.IsNullOrEmpty(user.LogoUrl))
                {
                    user.LogoUrl = string.Empty;
                }
            }

            await _account.SaveAsync();

            return Ok();
        }
    }
}
