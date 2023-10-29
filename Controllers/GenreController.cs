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
        //public DataContext _context { get; set; }

        public GenreController(IGenre genre) // DataContext context, 
        {
            //_context = context;
            _genre = genre;

        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<Genre>>> GetGenre()
        {
            var genres = await _genre.GetGenreAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(genres);
        }

        [HttpPost("{name}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<string>> AddGenre(string name)
        {
            await _genre.AddGenre(name);
            await _genre.Save();
            return Ok("Genre succecsully installed");
            
        }
    }
}
