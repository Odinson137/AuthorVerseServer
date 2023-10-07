using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface IGenre
    {
        Task<ICollection<Genre>> GetGenreAsync();
        Task Save();
        Task AddGenre(string name);
    }
}
