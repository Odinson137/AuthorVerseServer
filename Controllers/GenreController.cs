using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        public DataContext _context { get; set; }

        public GenreController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ICollection<GenreDTO>> GetGenre()
        {
            var genres = await _context.Genres.AsNoTracking().Select(genre => new GenreDTO()
            {
                GenreId = genre.GenreId,
                Name = genre.Name
            }).ToListAsync();

            return genres;
        }

        [HttpPost("{name}")]
        public async Task<string> AddGenre(string name)
        {
            await _context.Genres.AddAsync(new Genre()
            {
                Name = name
            });

            return "Genre succecsully installed";
            
        }
    }
}
