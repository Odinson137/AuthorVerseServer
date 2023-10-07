using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class GenreRepository : IGenre
    {
        private readonly DataContext _context;
        public GenreRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Genre>> GetGenreAsync()
        {
            return await _context.Genres.OrderBy(g => g.GenreId).ToListAsync();
        }

        public async Task AddGenre(string name)
        {
            await _context.Genres.AddAsync(new Genre()
            {
                Name = name
            });
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
