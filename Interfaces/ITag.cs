using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface ITag
    {
        Task<ICollection<Tag>> GetTagAsync();
        Task Save();
        Task AddTag(string name);
    }
}
