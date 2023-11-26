using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface IGenre
    {
        Task<ICollection<GenreDTO>> GetGenreAsync();
        Task Save();
        Task AddGenre(string name);
    }
}
