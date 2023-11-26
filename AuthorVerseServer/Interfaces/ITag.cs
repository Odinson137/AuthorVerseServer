using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface ITag
    {
        Task<ICollection<TagDTO>> GetTagAsync();
        Task Save();
        Task AddTag(string name);
    }
}
