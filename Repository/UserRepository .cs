using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class UserRepository : IUser
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<User>> GetUserAsync()
        {
            return await _context.Users.OrderBy(u => u.Id).ToListAsync();
        }
    }
}

