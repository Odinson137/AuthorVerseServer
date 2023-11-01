using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly IGenre _genre;
        private readonly IMemoryCache _cache;

        public GenreController(IGenre genre, IMemoryCache cache) 
        {
            _genre = genre;
            _cache = cache;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<GenreDTO>>> GetGenre()
        {
            var genres = await _cache.GetOrCreateAsync("genres", async entry =>
            {
                var genresDb = await _genre.GetGenreAsync();
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return genresDb;
            });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(genres);
        }

        [HttpPost("{name}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> AddGenre(string name)
        {
            await _genre.AddGenre(name);
            await _genre.Save();

            _cache.Remove("genres");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok("Genre succecsully installed");
        }
    }
}
