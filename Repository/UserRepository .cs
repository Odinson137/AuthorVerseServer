using AuthorVerseServer.Data;
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
        public UserRepository(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<User?> GetUser(string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<(bool, User)> CreateUser(GoogleJsonWebSignature.Payload info)
        {
            string userName = IsValidUsername(info.GivenName) ? info.GivenName : RemoveSpecialCharacters(info.GivenName);
            User user = new User()
            {
                UserName = userName,
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

        private static bool IsValidUsername(string username)
        {
            // Регулярное выражение для проверки строки на буквы или цифры
            Regex regex = new Regex("^[a-zA-Z0-9]*$");

            // Проверка строки на соответствие регулярному выражению
            return regex.IsMatch(username);
        }

        private static string RemoveSpecialCharacters(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
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

