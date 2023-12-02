using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class ForumMessageRepository : IForumMessage
    {
        private readonly DataContext _context;
        public ForumMessageRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddForumMessageAsync(ForumMessage message)
        {
            await _context.ForumMessages.AddAsync(message);
        }

        public async Task<ICollection<ForumMessageDTO>> GetForumMessagesAsync(int bookId, int part)
        {
            var messages = await _context.ForumMessages
                .AsNoTracking()
                .OrderByDescending(x => x.SendTime)
                .Where(x => x.BookId == bookId)
                .Skip(part * 30)
                .Take(30)
                .Select(x => new ForumMessageDTO()
                {
                    MessageId = x.MessageId,
                    Text = x.Text,
                    ViewName = string.IsNullOrEmpty(x.User.Name) ? x.User.UserName : $"{x.User.UserName} {x.User.LastName}",
                    SendTime = x.SendTime,
                }).ToArrayAsync();
            return messages;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
