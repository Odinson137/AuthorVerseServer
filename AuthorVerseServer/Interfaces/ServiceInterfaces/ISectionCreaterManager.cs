using AuthorVerseServer.DTO;
using StackExchange.Redis;

namespace AuthorVerseServer.Interfaces.ServiceInterfaces
{
    public interface ISectionCreateManager
    {
        ValueTask<ICollection<string>?> CreateManagerAsync(string userId, int chapterId);
        Task ManagerSaveAsync(string userId);
        ValueTask<string> DeleteSectionAsync(string userId, int number, int flow);
        ValueTask<string> CreateTextSectionAsync(string userId, int number, int flow, string text);
        ValueTask<string> CreateImageSectionAsync(string userId, int number, int flow, IFormFile file);
    }
}
