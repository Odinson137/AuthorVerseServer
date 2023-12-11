using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthorVerseServer.Interfaces
{
    public interface IForumMessage
    {
        Task SaveAsync();
        Task<ForumMessage> GetForumMessageAsync(int messageId);
        Task<ICollection<ForumMessageDTO>> GetForumMessagesAsync(int bookId, int part);
        Task<ICollection<ForumMessageDTO>> GetToParentMessagesAsync(int bookId, int lastMessageId, int parentMessageId);
        Task<int> AddForumMessageProcedureAsync(SendForumMessageDTO message);
        Task<bool> CheckUserMessageExistAsync(int messageId, string userId);
        Task DeleteMessageAsync(int messageId);
        Task ChangeParentMessage(int messageId);
        IDbContextTransaction StartTransaction();
    }
}
