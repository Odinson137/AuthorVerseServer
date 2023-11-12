using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController :  ControllerBase
    {
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
        public async Task<ActionResult<ICollection<CommentProfileDTO>>> GetUserComments(
            CommentType commentType = CommentType.All, int page = 1, string searchComment = "")
        {
            // userId from token
            return Ok(new List<CommentProfileDTO>());
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
    }
}
