using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace AuthorVerseServer.Repository
{
    public class UserRepository : IUser
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        private static string allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

        public UserRepository(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<User?> GetUserByUserName(string userName)
        {
            User? user = await _userManager.FindByNameAsync(userName);
            return user;
        }

        public async Task<(bool, User)> CreateGoogleUser(GoogleJsonWebSignature.Payload info)
        {
            User user = new User()
            {
                UserName = GenerateRandomUsername(),
                Name = info.GivenName,
                LastName = info.FamilyName,
                Email = info.Email,
                EmailConfirmed = true,
                Logo = new Image()
                {
                    Url = info.Picture
                }
            };

            var result = await _userManager.CreateAsync(user);
            return (result.Succeeded, user);
        }

        public static string GenerateRandomUsername()
        {
            const string prefix = "User";

            Random random = new Random();
            StringBuilder usernameBuilder = new StringBuilder(prefix);

            for (int i = 0; i < 10; i++)
            {
                char randomChar = allowedCharacters[random.Next(allowedCharacters.Length)];
                usernameBuilder.Append(randomChar);
            }

            return usernameBuilder.ToString();
        }

        public async Task<(bool, User)> CreateMicrosoftUser(UserProfile info)
        {
            User user = new User()
            {
                UserName = GenerateRandomUsername(),
                Name = info.GivenName,
                LastName = info.Surname,
            };

            var result = await _userManager.CreateAsync(user);
            return (result.Succeeded, user);
        }

        public async Task<ICollection<User>> GetUserAsync()
        {
            return await _context.Users.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

    }
}

