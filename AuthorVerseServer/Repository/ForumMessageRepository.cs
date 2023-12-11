using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace AuthorVerseServer.Repository
{
    public class ForumMessageRepository : IForumMessage
    {
        private readonly DataContext _context;
        public ForumMessageRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<int> AddForumMessageProcedureAsync(SendForumMessageDTO message)
        {
            int messageId = await _context.AddForumMessageAsync(
                bookId: message.BookId,
                userId: message.UserId,
                parentMessageId: message.AnswerId,
                text: message.Text);

            return messageId;
        }


        public async Task<bool> CheckUserMessageExistAsync(int messageId, string userId)
        {
            bool value = await _context.ForumMessages
                .AnyAsync(message => message.UserId == userId && message.MessageId == messageId);
            return value;
        }

        public async Task DeleteMessageAsync(int messageId)
        {
            await _context.ForumMessages
                .Where(message => message.MessageId == messageId)
                .ExecuteDeleteAsync();
        }


        public async Task ChangeParentMessage(int messageId)
        {
            await _context.ForumMessages
                .Where(message => message.ParrentMessageId == messageId)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(b => b.ParrentMessageId, (int?)null));
        }

        public async Task<ForumMessage> GetForumMessageAsync(int messageId)
        {
            var message = await _context.ForumMessages
                .Where(message => message.MessageId == messageId)
                .FirstAsync();
            return message;
        }

        public async Task<ICollection<ForumMessageDTO>> GetForumMessagesAsync(int bookId, int part)
        {
            var messages = await _context.ForumMessages
                .AsNoTracking()
                .OrderByDescending(x => x.SendTime)
                .Where(x => x.BookId == bookId)
                .Skip(part * 30)
                .Take(30)
                .Select(x => new ForumMessageDTO(x.User.Name, x.User.LastName, x.User.UserName)
                {
                    MessageId = x.MessageId,
                    Text = x.Text,
                    ParrentMessageId = x.ParrentMessageId,
                    SendTime = x.SendTime,
                }).ToArrayAsync();
            return messages;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IDbContextTransaction StartTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public async Task<ICollection<ForumMessageDTO>> GetToParentMessagesAsync(int bookId, int lastMessageId, int parentMessageId)
        {
            var messages = _context.ForumMessages
                    .Where(message => message.MessageId > lastMessageId && message.MessageId <= parentMessageId)
                    .Select(message => new ForumMessageDTO(message.User.Name, message.User.LastName, message.User.UserName)
                    { 
                        MessageId = message.MessageId,
                        Text = message.Text,
                        ParrentMessageId = message.ParrentMessageId,
                        SendTime = message.SendTime,
                    });

            return await messages.ToListAsync();
        }
    }
}
