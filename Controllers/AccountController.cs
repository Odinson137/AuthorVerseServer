using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AuthorVerseServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController :  ControllerBase
    {
        private readonly IAccount _account;
        private readonly UserManager<User> _userManager;
        private readonly MailService _mailService;
        private readonly CreateJWTtokenService _jWTtokenService;
        private readonly IMemoryCache _cache;
        public AccountController(
            IAccount account, UserManager<User> userManager, MailService mailService, CreateJWTtokenService jWTtokenService, IMemoryCache cache)
        {
            _account = account;
            _userManager = userManager;
            _mailService = mailService;
            _jWTtokenService = jWTtokenService;
            _cache = cache;
        }

        [HttpGet("Profile")]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile()
        {
            // userId from token
            return Ok(new UserProfileDTO());
        }

        [HttpGet("SelectedBooks")]
        public async Task<ActionResult<ICollection<UserSelectedBookDTO>>> GetSelectedBooks()
        {
            // userId from token
            return Ok(new List<UserSelectedBookDTO>());
        }

        [HttpGet("UserComments")]
        public async Task<ActionResult<CommentPageDTO>> GetUserComments(
            CommentType commentType = CommentType.All, int page = 1, string searchComment = "") // показывать на одной странице по 10 комментов
        {
            var a = Enum.IsDefined(typeof(CommentType), commentType);
            // userId from token
            return Ok(new CommentPageDTO());
        }

        [HttpGet("Friends")]
        public async Task<ActionResult<ICollection<FriendDTO>>> GetFriends()
        {
            // userId from token
            return Ok(new List<FriendDTO>());
        }

        [HttpGet("UserBooks")]
        public async Task<ActionResult<ICollection<UserBookDTO>>> GetUserBooks()
        {
            // userId from token
            return Ok(new List<UserBookDTO>());
        }

        [HttpGet("Updates")]
        public async Task<ActionResult<ICollection<UpdateAccountBook>>> GetUserBooksUpdates()
        {
            // userId from token
            return Ok(new List<UpdateAccountBook>());
        }
    }
}
