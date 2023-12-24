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

        public Task<List<GenreDTO>> GetGenreAsync()
        {
            var genres = _context.Genres.Select(genre => new GenreDTO
            {
                GenreId = genre.GenreId,
                Name = genre.Name,
            });

            return genres.ToListAsync();
        }

        public Task AddGenre(string name)
        {
            _context.Genres.AddAsync(new Genre()
            {
                Name = name
            });

            return Task.CompletedTask;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
