using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface IForumMessage
    {
        Task SaveAsync();
        Task<ForumMessage> GetForumMessageAsync(int messageId);
        Task<ICollection<ForumMessageDTO>> GetForumMessagesAsync(int bookId, int part);
        Task AddForumMessageAsync(ForumMessage message);
        Task<int> AddForumMessageProcedureAsync(SendForumMessageDTO message);
        Task<bool> CheckUserMessageExistAsync(int messageId, string userId);
        Task DeleteMessageAsync(int messageId);
        Task ChangeParentMessage(int messageId);
    }
}
