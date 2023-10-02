using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class UserSelectedBookRepository : IUserSelectedBook
    {
        private readonly DataContext _context;
        public UserSelectedBookRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<UserSelectedBook>> GetUserSelectedBookAsync()
        {
            return await _context.UserSelectedBooks.ToListAsync();
        }
    }
}
