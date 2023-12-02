using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface IForumMessage
    {
        Task SaveAsync();
        Task<ICollection<ForumMessageDTO>> GetForumMessagesAsync(int bookId, int part);
        Task AddForumMessageAsync(ForumMessage message);
    }
}
