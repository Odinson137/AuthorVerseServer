using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookChapterController : ControllerBase
    {
        private readonly IBookChapter _bookChapter;

        public BookChapterController(IBookChapter bookChapter)
        {
            _bookChapter = bookChapter;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<BookChapter>>> GetBookChapter()
        {
            var bookChapters = await _bookChapter.GetBookChapterAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(bookChapters);//Ок нужен чтобы работал код
        }
        
}
}
