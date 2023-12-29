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
        private readonly ILoadFile _loadImage;

        public AccountController(
            IAccount account, CreateJWTtokenService jWTtokenService, UserManager<User> userManager, ILoadFile loadImage)
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
                return Unauthorized("Token user is not correct");

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
                return Unauthorized("Token user is not correct");

            return Ok(await _account.GetUserSelectedBooksAsync(userId));
        }

        [HttpGet("UserComments")]
        public async Task<ActionResult<CommentPageDTO>> GetUserComments(
            CommentType commentType = CommentType.All, int page = 1, string searchComment = "") // показывать на одной странице по 10 комментов
        {
            if (!Enum.IsDefined(typeof(CommentType), commentType))
            {
                return BadRequest("CommentType is not correct");
            }

            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Token user is not correct");

            if (--page < 0)
                return BadRequest("Page is smaller than zero");
            
           //int commentsCount = page == 0 ? await _account.GetCommentsPagesCount(commentType, searchComment, userId) : 0;

            var commentPage = await _account.GetUserCommentsAsync(commentType, page, searchComment, userId);
            return Ok(commentPage);
        }

        [HttpGet("Friends")]
        public async Task<ActionResult<ICollection<FriendDTO>>> GetFriends()
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Token user is not correct");

            var friends = await _account.GetUserFriendsAsync(userId);

            return Ok(friends);
        }

        [HttpGet("UserBooks")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult<ICollection<UserBookDTO>>> GetUserBooks()
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Token user is not correct");

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
                return Unauthorized("Token user is not correct");

            var books = await _account.CheckUserUpdatesAsync(userId);

            return Ok(books);
        }

        [HttpPut("ProfileChange")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorMessageDTO))]
        [ProducesResponseType(404, Type = typeof(ErrorMessageDTO))]
        public async Task<ActionResult> ChangeUserProfile([FromBody] EditProfileDTO profile)
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Token user is not correct");

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
                await _loadImage.CreateFileAsync(profile.Logo, nameFile, "Images");
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
