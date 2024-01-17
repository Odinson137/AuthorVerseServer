using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthorVerseServer.Interfaces
{
    public interface IForumMessage
    {
        Task SaveAsync();
        Task<ForumMessage?> GetForumMessageAsync(int messageId);
        Task<List<ForumMessageDTO>> GetForumMessagesAsync(int bookId, int part);
        Task<List<ForumMessageDTO>> GetToParentMessagesAsync(int bookId, int lastMessageId, int parentMessageId);
        Task<int> AddForumMessageProcedureAsync(SendForumMessageDTO message);
        Task<bool> CheckUserMessageExistAsync(int messageId, string userId);
        Task<int> DeleteMessageAsync(int messageId);
        Task<int> ChangeParentMessage(int messageId);
        IDbContextTransaction StartTransaction();
        Task<int> ChangeMessageTextAsync(int messageId, string newText);
    }
}
