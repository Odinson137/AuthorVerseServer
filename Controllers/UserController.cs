using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Xml.Linq;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly SignInManager<User> _signInManager;
        public UserController(IUser user, SignInManager<User> signInManager)
        {
            _user = user;
            _signInManager = signInManager;
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

        // Настройка параметров аутентификации Google
        //var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Google",
        //    Url.Action("signin-google", "User", null, Request.Scheme));

        //// Добавление запрошенных разрешений к scope
        //authenticationProperties.Parameters.Add("scope", "openid profile email");

        //// Инициирование аутентификации через Google
        //return Challenge(authenticationProperties, "Google");

        [HttpGet("google")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("signin-google", "User", null, Request.Scheme);

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            // Генерация параметра correlation
            //properties.Items["correlation"] = Guid.NewGuid().ToString("N");

            return Challenge(properties, "Google");
        }

        [HttpPost("signin-google")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<bool> GoogleSignInCallback()
        {
            Console.WriteLine("Check");
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return false;
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }
    }
}
