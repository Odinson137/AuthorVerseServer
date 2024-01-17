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

        public Task<int> AddForumMessageProcedureAsync(SendForumMessageDTO message)
        {
            Task<int> messageId = _context.AddForumMessageAsync(
                bookId: message.BookId,
                userId: message.UserId,
                parentMessageId: message.AnswerId,
                text: message.Text);

            return messageId;
        }


        public Task<bool> CheckUserMessageExistAsync(int messageId, string userId)
        {
            var value = _context.ForumMessages
                .AnyAsync(message => message.UserId == userId && message.MessageId == messageId);
            return value;
        }

        public Task<int> DeleteMessageAsync(int messageId)
        {
            return _context.ForumMessages
                .Where(message => message.MessageId == messageId)
                .ExecuteDeleteAsync();
        }


        public Task<int> ChangeParentMessage(int messageId)
        {
            return _context.ForumMessages
                .Where(message => message.ParrentMessageId == messageId)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(b => b.ParrentMessageId, (int?)null));
        }

        public Task<ForumMessage?> GetForumMessageAsync(int messageId)
        {
            var message = _context.ForumMessages
                .Where(message => message.MessageId == messageId)
                .FirstOrDefaultAsync();
            return message;
        }

        public Task<List<ForumMessageDTO>> GetForumMessagesAsync(int bookId, int part)
        {
            var messages = _context.ForumMessages
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
                }).ToListAsync();
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

        public Task<int> ChangeMessageTextAsync(int messageId, string newText)
        {
            return _context.ForumMessages
                .Where(m => m.MessageId == messageId)
                .ExecuteUpdateAsync(c => 
                    c.SetProperty(m => m.Text, newText));
        }

        public Task<List<ForumMessageDTO>> GetToParentMessagesAsync(int bookId, int lastMessageId, int parentMessageId)
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

            return messages.ToListAsync();
        }
    }
}
