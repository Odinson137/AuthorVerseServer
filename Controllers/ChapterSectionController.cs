using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChapterSectionController : ControllerBase
    {
        private readonly IChapterSection _chapterSection;

        public ChapterSectionController(IChapterSection chapterSection)
        {
            _chapterSection = chapterSection;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<ChapterSection>>> GetChapterSection()
        {
            var chapterSections = await _chapterSection.GetChapterSectionAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(chapterSections);
        }
    }
}
