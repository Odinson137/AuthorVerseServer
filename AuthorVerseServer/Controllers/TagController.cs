using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITag _tag;
        private readonly IDistributedCache _redis;

        public TagController(ITag tag, IDistributedCache cache)
        {
            _tag = tag;
            _redis = cache;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<TagDTO>>> GetTag()
        {
            var tags = await _redis.GetStringAsync("tags");

            if (tags == null)
            {
                var tagsDb = await _tag.GetTagAsync();

                await _redis.SetStringAsync("tags", JsonConvert.SerializeObject(tagsDb), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });

                return Ok(tagsDb);
            }

            var deserializedTags = JsonConvert.DeserializeObject<List<TagDTO>>(tags);
            return Ok(deserializedTags);
        }

        [HttpPost("{name}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> AddGenre(string name)
        {
            await _tag.AddTag(name);
            await _tag.Save();

            _redis.Remove("tags");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok("Genre successfully installed");

        }
    }
}
