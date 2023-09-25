using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookChapterController : ControllerBase
    {
        private readonly IBookChapter _bookChapter;
        private readonly DataContext _context;
        public BookChapterController(DataContext context, IBookChapter bookChapter)
        {
            _context = context;
            _bookChapter = bookChapter;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<BookChapterDTO>>> GetBookChapter()
        {
            var bookChapters = await _bookChapter.GetBookChapterAsync();

            if (bookChapters == null)
                return NotFound();

            return Ok(bookChapters);
        }
    }
}
