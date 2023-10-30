using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthorVerseServer.Services;
using MimeKit;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using AuthorVerseServer.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly MailService _mailService;
        private readonly CreateJWTtokenService _jWTtokenService;

        public UserController(IUser user, IConfiguration configuration, UserManager<User> userManager, MailService mailService, CreateJWTtokenService jWTtokenService)
        {
            _user = user;
            _configuration = configuration;
            _mailService = mailService;
            _jWTtokenService = jWTtokenService;
            _userManager = userManager;
        }

        [HttpPost("Gmail")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> SendEmail([FromBody]UserRegistrationDTO user)
        {
            var token = _jWTtokenService.GenerateJwtTokenEmail(user, _configuration);
            string result = await _mailService.SendEmail(token, user.Email);
            return Ok(new MessageDTO { message = result });
        }

        [HttpGet("JWTHandler")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DecryptToken(string JWTToken)
        {
            var TokenInfo = new Dictionary<string, string>();

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(JWTToken);

            var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tokenExp)).UtcDateTime;

            if (tokenDate >= DateTime.Now.ToUniversalTime())
            {
                var claims = jwtSecurityToken.Claims.ToList();

                foreach (var claim in claims)
                {
                    TokenInfo.Add(claim.Type, claim.Value);
                }

                User newUser = new User()
                {
                    UserName = claims[0].Value,
                    Email = claims[1].Value,
                    Method = RegistrationMethod.Email
                };
                var result = await _userManager.CreateAsync(newUser, claims[2].Value);

                if(result.Succeeded)
                    return Ok(result);
                else
                    return BadRequest(result);
            } 
            else
                return BadRequest(new MessageDTO { message = "Token lifetime has run out" });
        }


        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<User>>> GetUser()
        {
            var users = await _user.GetUserAsync();

            if (!ModelState.IsValid)
                return BadRequest(new MessageDTO { message = "No users found" });

            return Ok(users);
        }

        [HttpPost("Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserVerify>> Login(UserLoginDTO authUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            User? user = await _userManager.FindByNameAsync(authUser.UserName);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, authUser.Password);
                if (passwordCheck)
                {
                    var Token = _jWTtokenService.GenerateJwtToken(user, _configuration);
                    return Ok(new UserVerify()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Token = Token
                    });
                }
                return BadRequest(new MessageDTO { message = "Password is not correct" });
            }
            return BadRequest(new MessageDTO { message = "User do not exist" });
        }

        [HttpPost("Registration")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> Registration(UserRegistrationDTO registeredUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            User? checkUser = await _userManager.FindByNameAsync(registeredUser.UserName);
            
            if (checkUser != null)
            {
                return BadRequest(new MessageDTO { message = "Thisn name is alredy taken" });
            }

            var validators = _userManager.PasswordValidators;

            bool resultOfValidation = true;
            foreach (var validator in validators)
            {
                var passCorrect = await validator.ValidateAsync(_userManager, null, registeredUser.Password);
                //В result указывается почему пароль не подходит

                if (!passCorrect.Succeeded)
                {
                    resultOfValidation = false;
                    break;
                }
            }

            if (resultOfValidation == true)
            {
                var newUser = new UserRegistrationDTO()//Далее этот userDTO переходит в SendEmail()
                {
                    UserName = registeredUser.UserName,
                    Email = registeredUser.Email,
                    Password = registeredUser.Password,
                };

                SendEmail(newUser);

                return Ok();
            }
            else
                return BadRequest(new MessageDTO { message = "Password type is not correct" });
         }

        [HttpPost("reg-google")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(MessageDTO))]
        public async Task<ActionResult<UserGoogleVerify>> RegWithGoogle([FromBody] AuthRequestModel token)
        {
            var userInfo = DecodeGoogleTokenService.VerifyGoogleIdToken(token.Token);

            User? user = await _userManager.FindByNameAsync(userInfo.Email);
            if (user != null)
            {
                return BadRequest( new MessageDTO { message = "This user has already existed" });
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
                return BadRequest(new MessageDTO { message = "Failed to create user" });
            }

            UserGoogleVerify userGoogle = new UserGoogleVerify()
            {
                Id = createUser.Id,
                UserName = createUser.UserName,
                Token = _jWTtokenService.GenerateJwtToken(createUser, _configuration),
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
                return BadRequest(new MessageDTO { message = "User not found" });
            }

            UserGoogleVerify userGoogle = new UserGoogleVerify()
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = _jWTtokenService.GenerateJwtToken(user, _configuration),
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
                return BadRequest(new MessageDTO { message = "User not found" });
            }

            User user = microsoftUser.User;

            UserMicrosoftVerify userGoogle = new UserMicrosoftVerify()
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = _jWTtokenService.GenerateJwtToken(user, _configuration),
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
                return BadRequest(new MessageDTO { message = "User has already exist" });
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
                return BadRequest(new MessageDTO { message = "Failed to create user" });
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
                Token = _jWTtokenService.GenerateJwtToken(user, _configuration),
            };

            return Ok(userMicrosoft);
        }
    }

}
