using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookChapterController : ControllerBase
    {
        private readonly IBookChapter _bookChapter;
        private readonly CreateJWTtokenService _tokenService;
        private readonly MailService _mailService;

        public BookChapterController(IBookChapter bookChapter, CreateJWTtokenService tokenService, MailService mailService)
        {
            _bookChapter = bookChapter;
            _tokenService = tokenService;
            _mailService = mailService;
        }

        // can be authorize
        [HttpGet("Read")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<PageBookChaptersDTO>> GetBookChapter(int bookId)
        {
            var bookChapters = await _bookChapter.GetBookReadingChaptersAsync(bookId);

            string? userId = _tokenService.GetIdFromToken(this.User);

            int chapterNumber = 0;
            if (!string.IsNullOrEmpty(userId))
            {
                chapterNumber = await _bookChapter.GetUserReadingNumberAsync(bookId, userId);
            }

            var userChapters = new PageBookChaptersDTO
            {
                LastReadingNumber = chapterNumber,
                BookChapters = bookChapters,
            };

            return Ok(userChapters);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<int>> AddNewChapter(string title, int lastChapterId)
        {
            string? userId = _tokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token failed");
            }

            ChapterInfo? chapterInfo = await _bookChapter.GetChapterNumberAsync(lastChapterId, userId);
            if (chapterInfo == null)
            {
                return NotFound("Chapter not found");
            }

            var chapter = new BookChapter()
            {
                BookId = chapterInfo.BookId,
                Title = title,
                BookChapterNumber = chapterInfo.ChapterNumber + 1,
            };

            await _bookChapter.AddNewChapterAsync(chapter);
            await _bookChapter.SaveAsync();

            return Ok(chapter.BookChapterId);
        }


        [Authorize]
        [HttpPost("Publicate/{chapterId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PublicateChapter(int chapterId)
        {
            string? userId = _tokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token failed");
            }

            await _bookChapter.PublicateChapterAsync(chapterId);

            // система уведомлений пользователям о новой вышедшей главе
            await _mailService.SendNotifyEmail(chapterId);

            return Ok();
        }

        [Authorize]
        [HttpPost("AuthorChapters")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<ShortAuthorChapterDTO>>> GetAuthorChaptersAsync(int bookId)
        {
            string? userId = _tokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token failed");
            }

            var chapters = await _bookChapter.GetAuthorChaptersAsync(bookId, userId);

            return Ok(chapters);
        }

        [Authorize]
        [HttpPost("DetailChapter")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<DetaildAuthorChapterDTO>>> GetAuthorDetaildChapterAsync(int bookId)
        {
            string? userId = _tokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token failed");
            }

            var chapter = await _bookChapter.GetAuthorDetaildChapterAsync(bookId, userId);

            if (chapter == null)
            {
                return NotFound("Chapter is not found");
            }

            return Ok(chapter);
        }

        [Authorize]
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateChapter(int chapterId, string? place = null, string? description = null)
        {
            string? userId = _tokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token failed");
            }

            var chapter = await _bookChapter.GetBookChapterAsync(chapterId, userId);

            if (chapter == null)
            {
                return NotFound("Chapter is not found");
            }

            if (!string.IsNullOrEmpty(place))
            {
                chapter.ActionPlace = place;
            }

            if (!string.IsNullOrEmpty(description))
            {
                chapter.Description = description;
            }

            await _bookChapter.SaveAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteChapter(int chapterId)
        {
            string? userId = _tokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token failed");
            }

            if (await _bookChapter.IsAuthorAsync(chapterId, userId) == false)
            {
                return NotFound("Chapter is not found");
            }

            if (await _bookChapter.AnyChildExistAsync(chapterId) == true)
            {
                return BadRequest("This chapter is not last");
            }

            await _bookChapter.DeleteChapterAsync(chapterId);

            return Ok();
        }
    }
}
