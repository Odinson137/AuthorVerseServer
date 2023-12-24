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

        public Task CreateMicrosoftUser(MicrosoftUser microsoftUser)
        {
            _context.MicrosoftUsers.AddAsync(microsoftUser);
            return Task.CompletedTask;
        }

        public Task<MicrosoftUser?> GetMicrosoftUser(string azureName)
        {
            return _context.MicrosoftUsers.Include(x => x.User).FirstOrDefaultAsync(x => x.AzureName == azureName);
        }

        public Task<List<string>> GetUserEmailAsync(int bookId)
        {
            var emails = _context.UserSelectedBooks
                .Where(b => b.BookId == bookId)
                // возможно исключить автора сие творения из выборки
                .Where(b => b.User.EmailConfirmed == true)
                .Select(b => b.User.Email!);

            return emails.ToListAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}

