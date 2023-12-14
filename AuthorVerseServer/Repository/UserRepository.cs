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

        public async Task CreateMicrosoftUser(MicrosoftUser microsoftUser)
        {
            await _context.MicrosoftUsers.AddAsync(microsoftUser);
        }

        public async Task<MicrosoftUser?> GetMicrosoftUser(string azureName)
        {
            return await _context.MicrosoftUsers.Include(x => x.User).FirstOrDefaultAsync(x => x.AzureName == azureName);
        }

        public async Task<ICollection<string>> GetUserEmailAsync(int bookId)
        {
            var emails = _context.Books
                .Where(b => b.BookId == bookId)
                // возможно исключить автора сие творения из выборки
                .Where(b => b.Author.EmailConfirmed == true)
                .Select(b => b.Author.Email!);

            return await emails.ToListAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}

