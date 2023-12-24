using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface IGenre
    {
        Task<List<GenreDTO>> GetGenreAsync();
        Task Save();
        Task AddGenre(string name);
    }
}
