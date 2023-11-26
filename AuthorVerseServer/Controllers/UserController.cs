using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using AuthorVerseServer.Data.Enums;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly UserManager<User> _userManager;
        private readonly MailService _mailService;
        private readonly CreateJWTtokenService _jWTtokenService;
        private readonly GenerateRandomNameService _generateNameService;
        private readonly IDatabase _redis;
        public UserController(
            IUser user, IConnectionMultiplexer redisConnection,
            UserManager<User> userManager, MailService mailService, 
            CreateJWTtokenService jWTtokenService, GenerateRandomNameService generateRandomName)
        {
            _user = user;
            _redis = redisConnection.GetDatabase();
            _mailService = mailService;
            _jWTtokenService = jWTtokenService;
            _userManager = userManager;
            _generateNameService = generateRandomName;
        }

        [HttpGet("GetSuperUnSafeTokenOperation")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<string>> GetTokenAsync()
        {
            var token = _jWTtokenService.GenerateAdminJwtToken("admin");
            return Ok(token);
        }

        [HttpPost("Gmail")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> SendEmail([FromBody] UserRegistrationDTO user)
        {
            string result = await Send(user);
            return Ok(new MessageDTO { Message = result });
        }

        private async Task<string> Send(UserRegistrationDTO user)
        {
            string jsonUSer = JsonConvert.SerializeObject(user);
            await _redis.StringSetAsync($"user:{user.UserName}", jsonUSer, TimeSpan.FromMinutes(15));

            var token = _jWTtokenService.GenerateJwtTokenEmail(user);
            string result = await _mailService.SendEmail(token, user.Email);
            return result;
        }

        [HttpPost("EmailConfirm/{token}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(MessageDTO))]
        public async Task<ActionResult<bool>> DecryptToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var tokenExp = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type.Equals("exp"))?.Value;
            if (tokenExp == null || !long.TryParse(tokenExp, out long expUnixTime))
            {
                return BadRequest(new MessageDTO ("Invalid token"));
            }

            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(expUnixTime).UtcDateTime;
            if (tokenDate < DateTime.Now.ToUniversalTime())
            {
                return BadRequest(new MessageDTO ("Token lifetime has run out"));
            }
            var userName = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type.Equals("unique_name"))?.Value;

            string jsonUser = await _redis.StringGetAsync($"user:{userName}");
            var user = JsonConvert.DeserializeObject<UserRegistrationDTO>(jsonUser);

            if (user == null)
            {
                return BadRequest(new MessageDTO ("Token lifetime has run out"));
            }

            User newUser = new User()
            {
                UserName = userName,
                Email = user.Email,
                Method = RegistrationMethod.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new MessageDTO(string.Join(", ", result.Errors)));
            }
        }


        [HttpPost("Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginDTO authUser)
        {
            User? user = await _userManager.FindByNameAsync(authUser.UserName);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, authUser.Password);
                if (passwordCheck)
                {
                    var token = _jWTtokenService.GenerateJwtToken(user.Id);
                    var verifyUser = new UserVerify()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Name = user.Name,
                        LastName = user.LastName,
                        LogoUrl = user.LogoUrl,
                    };

                    await _redis.StringSetAsync($"session-{verifyUser.Id}", JsonConvert.SerializeObject(verifyUser));
                    return Ok(token);
                }
                return BadRequest(new MessageDTO("Password is not correct"));
            }
            return BadRequest(new MessageDTO("User do not exist"));
        }

        [HttpPost("Registration")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(MessageDTO))]
        public async Task<ActionResult<MessageDTO>> Registration(UserRegistrationDTO registeredUser)
        {
            User? checkUser = await _userManager.FindByNameAsync(registeredUser.UserName);
            var userCache = await _redis.StringGetAsync($"user:{registeredUser.UserName}");

            if (checkUser != null || !string.IsNullOrEmpty(userCache))
            {
                return BadRequest(new MessageDTO { Message = "This name is already taken" });
            }

            User? checkEmail = await _userManager.FindByEmailAsync(registeredUser.Email);

            if (checkEmail != null)
            {
                return BadRequest(new MessageDTO { Message = "This email is already taken" });
            }

            string result = await Send(registeredUser);

            return Ok(new MessageDTO { Message = result });
         }

        [HttpPost("reg-google")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(MessageDTO))]
        public async Task<ActionResult> RegWithGoogle([FromBody] AuthRequestModel token)
        {
            var userInfo = DecodeGoogleTokenService.VerifyGoogleIdToken(token.Token);
            if (userInfo == null) return BadRequest(new MessageDTO { Message = "Error token" });

            User? user = await _userManager.FindByNameAsync(userInfo.Email);
            if (user != null)
            {
                return BadRequest( new MessageDTO { Message = "This user has already existed" });
            }

            User createUser = new User()
            {
                UserName = _generateNameService.GenerateRandomUsername(),
                Name = userInfo.GivenName,
                LastName = userInfo.FamilyName,
                Email = userInfo.Email,
                Method = RegistrationMethod.Google,
                EmailConfirmed = true,
                LogoUrl = userInfo.Picture
            };

            var result = await _userManager.CreateAsync(createUser);

            if (!result.Succeeded)
            {
                return BadRequest(new MessageDTO { Message = "Failed to create user" });
            }

            return Ok();
        }

        [HttpPost("signin-google")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(MessageDTO))]
        public async Task<ActionResult<UserGoogleVerify>> SignInWithGoogle([FromBody] AuthRequestModel token)
        {
            var userInfo = DecodeGoogleTokenService.VerifyGoogleIdToken(token.Token);
            if (userInfo == null) return BadRequest(new MessageDTO { Message = "Error token" });

            User? user = await _userManager.FindByNameAsync(userInfo.Email);
            if (user == null)
            {
                return BadRequest(new MessageDTO { Message = "User not found" });
            }

            var jwToken = _jWTtokenService.GenerateJwtToken(user.Id);
            var verifyUser = new UserVerify()
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                LastName = user.LastName,
                LogoUrl = user.LogoUrl,
            };

            await _redis.StringSetAsync($"session-{user.Id}", JsonConvert.SerializeObject(verifyUser));
            return Ok(jwToken);
        }

        [HttpPost("signin-microsoft")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserVerify>> SignInWithMicrosoft([FromBody] UserProfile userInfo)
        {
            var microsoftUser = await _user.GetMicrosoftUser(userInfo.UserPrincipalName);
            if (microsoftUser == null)
            {
                return BadRequest(new MessageDTO { Message = "User not found" });
            }

            User user = microsoftUser.User;

            var jwToken = _jWTtokenService.GenerateJwtToken(user.Id);
            var verifyUser = new UserVerify()
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                LastName = user.LastName,
                LogoUrl = user.LogoUrl,
            };

            await _redis.StringSetAsync($"session-{user.Id}", JsonConvert.SerializeObject(verifyUser));
            return Ok(jwToken);
        }


        [HttpPost("reg-microsoft")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> RegWithMicrosoft([FromBody] UserProfile userInfo)
        {
            var microsoftUser = await _user.GetMicrosoftUser(userInfo.UserPrincipalName);
            if (microsoftUser != null)
            {
                return BadRequest(new MessageDTO { Message = "User has already exist" });
            }

            User user = new User()
            {
                UserName = _generateNameService.GenerateRandomUsername(),
                Name = userInfo.GivenName,
                LastName = userInfo.Surname,
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new MessageDTO { Message = "Failed to create user" });
            }

            await _user.CreateMicrosoftUser(new MicrosoftUser()
            {
                User = user,
                AzureName = userInfo.UserPrincipalName
            });

            await _user.Save();

            return Ok();
        }
    }

}
