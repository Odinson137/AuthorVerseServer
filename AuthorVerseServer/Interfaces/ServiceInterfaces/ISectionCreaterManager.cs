using StackExchange.Redis;

namespace AuthorVerseServer.Interfaces.ServiceInterfaces
{
    public interface ISectionCreateManager
    {
        ValueTask<SortedSetEntry[]?> CreateManagerAsync(string userId, int chapterId);
        ValueTask<string> CreateTextSectionAsync(string userId, int number, int flow, string text);
    }
}
