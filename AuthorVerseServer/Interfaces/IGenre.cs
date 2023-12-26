using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface IGenre
    {
        Task<List<GenreDTO>> GetGenreAsync();
        Task<int> SaveAsync();
        Task AddGenre(string name);
    }
}
