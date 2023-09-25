using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly IGenre _genre;

        public DataContext _context { get; set; }

        public GenreController(DataContext context, IGenre genre)
        {
            _context = context;
            _genre = genre;

        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<Genre>>> GetGenre()
        {
            var genres = await _genre.GetGenreAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(genres);
        }

        /*[HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<GenreDTO>>> GetGenre()
        {
            var genres = await _context.Genres.AsNoTracking().Select(genre => new GenreDTO()
            {
                GenreId = genre.GenreId,
                Name = genre.Name
            }).ToListAsync();

            return genres;
        }*/

        [HttpPost("{name}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<string>> AddGenre(string name)
        {
            await _context.Genres.AddAsync(new Genre()
            {
                Name = name
            });

            _context.SaveChanges();

            return "Genre succecsully installed";
            
        }
    }
}
