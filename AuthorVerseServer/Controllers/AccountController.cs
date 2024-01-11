using AuthorVerseServer.Data.ControllerSettings;
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
    public class AccountController :  AuthorVerseController
    {
        private readonly IAccount _account;
        private readonly UserManager<User> _userManager;
        private readonly LoadFileService _loadImage;

        public AccountController(
            IAccount account, UserManager<User> userManager, LoadFileService loadImage)
        {
            _account = account;
            _userManager = userManager;
            _loadImage = loadImage;
        }

        [HttpGet("Profile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404, Type = typeof(string))]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile()
        {
            UserProfileDTO profile = await _account.GetUserAsync(UserId);

            return Ok(profile);
        }

        [HttpGet("SelectedBooks")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult<ICollection<SelectedUserBookDTO>>> GetSelectedBooks()
        {
            return Ok(await _account.GetUserSelectedBooksAsync(UserId));
        }

        [HttpGet("UserComments")]
        public async Task<ActionResult<CommentPageDTO>> GetUserComments(
            CommentType commentType = CommentType.All, int page = 1, string searchComment = "") // показывать на одной странице по 10 комментов
        {
            if (!Enum.IsDefined(typeof(CommentType), commentType))
            {
                return BadRequest("CommentType is not correct");
            }

            if (--page < 0)
                return BadRequest("Page is smaller than zero");
            
           //int commentsCount = page == 0 ? await _account.GetCommentsPagesCount(commentType, searchComment, UserId) : 0;

            var commentPage = await _account.GetUserCommentsAsync(commentType, page, searchComment, UserId);
            return Ok(commentPage);
        }

        [HttpGet("Friends")]
        public async Task<ActionResult<ICollection<FriendDTO>>> GetFriends()
        {
            var friends = await _account.GetUserFriendsAsync(UserId);

            return Ok(friends);
        }

        [HttpGet("UserBooks")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult<ICollection<UserBookDTO>>> GetUserBooks()
        {
            var books = await _account.GetUserBooksAsync(UserId);
            return Ok(books);
        }

        [HttpGet("Updates")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult<ICollection<UpdateAccountBook>>> GetUserBooksUpdates()
        {
            var books = await _account.CheckUserUpdatesAsync(UserId);

            return Ok(books);
        }

        [HttpPut("ProfileChange")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorMessageDTO))]
        [ProducesResponseType(404, Type = typeof(ErrorMessageDTO))]
        public async Task<ActionResult> ChangeUserProfile([FromBody] EditProfileDTO profile)
        {
            var user = await _userManager.FindByIdAsync(UserId);
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
