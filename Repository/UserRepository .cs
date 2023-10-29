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
        

        public UserRepository(DataContext context)
        {
            _context = context;
            
        }

<<<<<<< HEAD
        //public async Task<User?> GetUserByEmail(string email)
        //{
        //    return 
        //}
        //public async Task<User?> GetUserByUsesrName(string name)
        //{
        //    return await _userManager.FindByNameAsync(name);
        //}
=======
        public async Task<bool> PasswordValidation(string password)
        {
            var validators = _userManager.PasswordValidators;

            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(_userManager, null, password);
                //В result указывается почему пароль не подходит

                if (!result.Succeeded)
                    return false;
            }
            return true;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userManager.FindByNameAsync(email);
        }
        public async Task<User?> GetUserByUsesrName(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }
>>>>>>> add Token lifeteme and changed registration

        //public async Task<IdentityResult> CreateUser(User newUser, string password)
        //{
        //    return await _userManager.CreateAsync(newUser, password);
        //}

        //public async Task<bool> CheckUserPassword(User user, string password)
        //{
        //    return await _userManager.CheckPasswordAsync(user, password);
        //}

        //public async Task<User?> GetUserByUserName(string userName)
        //{
        //    User? user = await _userManager.FindByNameAsync(userName);
        //    return user;
        //}

        //public async Task<bool> CreateForeignUser(User user)
        //{
        //    var result = await _userManager.CreateAsync(user);
        //    return result.Succeeded;
        //}

        public async Task CreateMicrosoftUser(MicrosoftUser microsoftUser)
        {
            await _context.MicrosoftUsers.AddAsync(microsoftUser);
        }

        public async Task<MicrosoftUser?> GetMicrosoftUser(string azureName)
        {
            return await _context.MicrosoftUsers.Include(x => x.User).FirstOrDefaultAsync(x => x.AzureName == azureName);
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

