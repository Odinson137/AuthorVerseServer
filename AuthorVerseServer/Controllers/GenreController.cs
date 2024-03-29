﻿using AsyncAwaitBestPractices;
using AuthorVerseServer.Data;
using AuthorVerseServer.Data.ControllerSettings;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : AuthorVerseController
    {
        private readonly IGenre _genre;
        private readonly IDistributedCache _redis;

        public GenreController(IGenre genre, IDistributedCache cache) 
        {
            _genre = genre;
            _redis = cache;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<GenreDTO>>> GetGenre()
        {
            var genres = await _redis.GetStringAsync("genres");

            if (genres == null)
            {
                var genresDb = await _genre.GetGenreAsync();

                await _redis.SetStringAsync("genres", JsonConvert.SerializeObject(genresDb), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                });

                return Ok(genresDb);
            }

            return Ok(genres);
        }

        [HttpPost("{name}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> AddGenre(string name)
        {
            await _genre.AddGenre(name);
            await _genre.SaveAsync();

            _redis.RemoveAsync("genres").SafeFireAndForget();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok("Genre successfully installed");
        }
    }
}
