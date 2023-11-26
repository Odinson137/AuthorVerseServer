using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
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

        public async Task<ICollection<GenreDTO>> GetGenreAsync()
        {
            var genres = _context.Genres.Select(genre => new GenreDTO
            {
                GenreId = genre.GenreId,
                Name = genre.Name,
            });

            return await genres.ToListAsync();
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
