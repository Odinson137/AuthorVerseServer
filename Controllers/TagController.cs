using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITag _tag;

        public TagController(ITag tag)
        {
            _tag = tag;

        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<TagDTO>>> GetTag()
        {
            var genres = await _tag.GetTagAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(genres);
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
