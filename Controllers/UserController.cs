using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthorVerseServer.Services;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IConfiguration _configuration;

        public UserController(IUser user, IConfiguration configuration)
        {
            _user = user;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<User>>> GetUser()
        {
            var users = await _user.GetUserAsync();

            if (!ModelState.IsValid)
                return BadRequest("No users found");

            return Ok(users);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<bool> Login()
        {
            return true;
        }

        [HttpPost("signin-google")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserGoogleVerify>> GoogleSignInCallback([FromBody] AuthRequestModel token)
        {
            var userInfo = DecodeGoogleTokenService.VerifyGoogleIdToken(token.Token);

            User? user = await _user.GetUser(userInfo.Email);
            if (user == null)
            {
                (bool result, User createUser) = await _user.CreateUser(userInfo);
                if (!result)
                {
                    return BadRequest("Failed to create user");
                }

                await _user.Save();
                user = createUser;
            }

            UserGoogleVerify userGoogle = new UserGoogleVerify()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = CreateJWTtokenService.GenerateJwtToken(user, _configuration),
                IconUrl = userInfo.Picture
            };

            return Ok(userGoogle);
        }


    }
}
