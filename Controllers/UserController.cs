using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthorVerseServer.Services;
using AuthorVerseServer.Enums;
using Microsoft.AspNetCore.Identity;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public UserController(IUser user, IConfiguration configuration, UserManager<User> userManager)
        {
            _user = user;
            _configuration = configuration;
            _userManager = userManager;
        }

        // не нужна для тестов
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<User>>> GetUser()
        {
            var users = await _user.GetUserAsync();

            if (!ModelState.IsValid)
                return BadRequest("No users found");

            return Ok(users);
        }

        [HttpPost("Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login(UserLoginDTO authUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            User? user = await _userManager.FindByNameAsync(authUser.UserName);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, authUser.Password);
                if (passwordCheck)
                {
                    var Token = CreateJWTtokenService.GenerateJwtToken(user, _configuration);
                    return Ok(new UserVerify()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Token = Token
                    });
                }
                return BadRequest("Password is not correct");
            }
            return BadRequest("User do not exist");
        }

        [HttpPost("Registration")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> Registration(UserRegistrationDTO registeredUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            User? checkUser = await _userManager.FindByNameAsync(registeredUser.UserName);
            //var checkUser = await _user.GetUserByUserName(registeredUser.UserName);
            
            if (checkUser != null)
            {
                return BadRequest("Thisn name is alredy taken");
            }

            var newUser = new User()
            {
                UserName = registeredUser.UserName,
                Email = registeredUser.Email,
                Method = RegistrationMethod.Email
            };

            var result = await _userManager.CreateAsync(newUser, registeredUser.Password);
            //var result = await _user.CreateUser(newUser, );

            if (!result.Succeeded)
                return BadRequest("Password type is incorrect");

            return Ok("User successfully created");
        }


        [HttpPost("reg-google")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserGoogleVerify>> RegWithGoogle([FromBody] AuthRequestModel token)
        {
            var userInfo = DecodeGoogleTokenService.VerifyGoogleIdToken(token.Token);

            User? user = await _userManager.FindByNameAsync(userInfo.Email);
            if (user != null)
            {
                return BadRequest("This user has already existed");
            }

            User createUser = new User()
            {
                UserName = GenerateRandomName.GenerateRandomUsername(),
                Name = userInfo.GivenName,
                LastName = userInfo.FamilyName,
                Email = userInfo.Email,
                Method = RegistrationMethod.Google,
                EmailConfirmed = true,
                LogoUrl = userInfo.Picture
            };

            var result = await _userManager.CreateAsync(createUser);

            //bool result = await _user.CreateForeignUser(createUser);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to create user");
            }

            UserGoogleVerify userGoogle = new UserGoogleVerify()
            {
                Id = createUser.Id,
                UserName = createUser.UserName,
                Token = CreateJWTtokenService.GenerateJwtToken(createUser, _configuration),
                IconUrl = userInfo.Picture
            };

            return Ok(userGoogle);
        }

        [HttpPost("signin-google")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserGoogleVerify>> SignInWithGoogle([FromBody] AuthRequestModel token)
        {
            var userInfo = DecodeGoogleTokenService.VerifyGoogleIdToken(token.Token);

            User? user = await _userManager.FindByNameAsync(userInfo.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            UserGoogleVerify userGoogle = new UserGoogleVerify()
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = CreateJWTtokenService.GenerateJwtToken(user, _configuration),
                IconUrl = userInfo.Picture
            };

            return Ok(userGoogle);
        }

        [HttpPost("signin-microsoft")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserMicrosoftVerify>> SignInWithMicrosoft([FromBody] UserProfile userInfo)
        {
            var microsoftUser = await _user.GetMicrosoftUser(userInfo.UserPrincipalName);
            if (microsoftUser == null)
            {
                return BadRequest("User not found");
            }

            User user = microsoftUser.User;

            UserMicrosoftVerify userGoogle = new UserMicrosoftVerify()
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = CreateJWTtokenService.GenerateJwtToken(user, _configuration),
            };

            return Ok(userGoogle);
        }


        [HttpPost("reg-microsoft")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserMicrosoftVerify>> RegWithMicrosoft([FromBody] UserProfile userInfo)
        {
            var microsoftUser = await _user.GetMicrosoftUser(userInfo.UserPrincipalName);
            if (microsoftUser != null)
            {
                return BadRequest("User has already exist");
            }

            User user = new User()
            {
                UserName = GenerateRandomName.GenerateRandomUsername(),
                Name = userInfo.GivenName,
                LastName = userInfo.Surname,
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to create user");
            }

            await _user.CreateMicrosoftUser(new MicrosoftUser()
            {
                User = user,
                AzureName = userInfo.UserPrincipalName
            });

            await _user.Save();

            UserMicrosoftVerify userMicrosoft = new UserMicrosoftVerify()
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = CreateJWTtokenService.GenerateJwtToken(user, _configuration),
            };

            return Ok(userMicrosoft);
        }
    }
}
