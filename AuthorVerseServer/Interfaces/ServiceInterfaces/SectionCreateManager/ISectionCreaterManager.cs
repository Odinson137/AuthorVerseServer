using AuthorVerseServer.DTO;
using StackExchange.Redis;

namespace AuthorVerseServer.Interfaces.ServiceInterfaces
{
    public interface ISectionCreateManager
    {
        ValueTask<ICollection<string>?> CreateManagerAsync(string userId, int chapterId);
        Task<int> ManagerSaveAsync(string userId);
        Task AddSectionToRedisAsync(string userId, int number, int flow, string serviceKey, object value);
        Task UpdateSectionToRedisAsync(string userId, int number, int flow, string serviceKey, object value);
        Task DeleteSectionFromRedisAsync(string userId, int number, int flow);
    }
}
