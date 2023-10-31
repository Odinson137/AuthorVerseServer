using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITag _tag;
        private readonly IMemoryCache _cache;

        public TagController(ITag tag, IMemoryCache cache)
        {
            _tag = tag;
            _cache = cache;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<GenreDTO>>> GetTag()
        {
            var tags = await _cache.GetOrCreateAsync("tags", async entry =>
            {
                var tagsDb = await _tag.GetTagAsync();
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return tagsDb;
            });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tags);
        }

        [HttpPost("{name}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> AddGenre(string name)
        {
            await _tag.AddTag(name);
            await _tag.Save();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok("Genre successfully installed");

        }
    }
}
