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

