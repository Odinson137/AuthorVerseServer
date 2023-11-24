﻿using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly IGenre _genre;
        private readonly IDistributedCache _cache;

        public GenreController(IGenre genre, IDistributedCache cache) 
        {
            _genre = genre;
            _cache = cache;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<GenreDTO>>> GetGenre()
        {
            var genres = await _cache.GetStringAsync("genres");

            if (genres == null)
            {
                var genresDb = await _genre.GetGenreAsync();

                await _cache.SetStringAsync("genres", JsonConvert.SerializeObject(genresDb), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });

                return Ok(genresDb);
            }

            var genresCache = JsonConvert.DeserializeObject<List<GenreDTO>>(genres);
            return Ok(genresCache);
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
