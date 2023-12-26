using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface ITag
    {
        Task<List<TagDTO>> GetTagAsync();
        Task<int> SaveAsync();
        Task AddTag(string name);
    }
}
