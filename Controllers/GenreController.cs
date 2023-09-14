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
        [ProducesResponseType(200, Type = typeof(ICollection<GenreDTO>))]
        public async Task<IActionResult> GetGenre()
        {
            var genres = await _context.Genres.AsNoTracking().Select(genre => new GenreDTO()
            {
                GenreId = genre.GenreId,
                Name = genre.Name
            }).ToListAsync();

            return Ok(genres);
        }

        [HttpPost("{name}")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> AddGenre(string name)
        {
            await _context.Genres.AddAsync(new Genre()
            {
                Name = name
            });

            return Ok("Genre succecsully installed");
            
        }
    }
}
