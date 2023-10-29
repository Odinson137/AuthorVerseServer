using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBook _book;
        private readonly UserManager<User> _userManager;

        public BookController(IBook book, UserManager<User> userManager)
        {
            _book = book;
            _userManager = userManager;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<Book>>> GetBooks()
        {
            var books = await _book.GetBooksAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(books);
        }

        [HttpGet("Count")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<int>> GetCountBooks()
        {
            int bookCount = await _book.GetCountBooks();
            return Ok(bookCount);
        }

        [HttpGet("Popular")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<PopularBook>>> GetPopularBooks()
        {
            return Ok(await _book.GetPopularBooks());
        }

        [HttpGet("Last")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<PopularBook>>> GetLastBooks()
        {
            return Ok(await _book.GetLastBooks());
        }

        [HttpGet("Page/{page?}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<BookDTO>>> GetSecrtainBooksPage(int page = 0)
        {
            return Ok(await _book.GetCertainBooksPage(page));
        }

        [HttpGet("{bookId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BookDTO>> GetBook(int bookId)
        {
            var book = await _book.GetBookById(bookId);

            if (book == null)
            {
                return NotFound("This book not found");
            }

            return book;
        }

        [HttpPost("Create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<int>> CreateBook([FromBody] CreateBookDTO bookDTO)
        {
            if (string.IsNullOrEmpty(bookDTO.AuthorId))
            {
                return BadRequest("Invalid user Id");
            }

            if (bookDTO.GenresId != null)
            {
                if (bookDTO.GenresId.Count == 0)
                {
                    return BadRequest("Genres are not select");
                }
            } else
            {
                return BadRequest("Genres are not select");
            }

            if (bookDTO.TagsId != null)
            {
                if (bookDTO.TagsId.Count == 0)
                    return BadRequest("Tags are not select");
            } else
            {
                return BadRequest("Tags are not select");
            }

            User? user = await _userManager.FindByIdAsync(bookDTO.AuthorId);

            if (user == null)
            {
                return NotFound("Author not found");
            }

            Book book = new Book()
            {
                Author = user,
                AuthorId = bookDTO.AuthorId,
                Title = bookDTO.Title,
                Description = bookDTO.Description,
                AgeRating = bookDTO.AgeRating,
                BookCover = bookDTO.BookCoverUrl
            };

            await _book.AddBook(book);

            foreach (var genreId in bookDTO.GenresId)
            {
                var genre = await _book.GetGenreById(genreId);

                if (genre != null)
                {
                    book.Genres.Add(genre);
                } else
                {
                    return NotFound("Genre not found");
                }
            }

            foreach (var tagId in bookDTO.TagsId)
            {
                var tag = await _book.GetTagById(tagId);

                if (tag != null)
                {
                    book.Tags.Add(tag);
                }
                else
                {
                    return NotFound("Tag not found");
                }
            }

            await _book.Save();

            if (!ModelState.IsValid)
            {
                return BadRequest("Book not create");
            }

            return Ok(book.BookId);
        }

        [HttpGet("MainPopularBooks")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<PopularBook>>> GetMainPopularBooks()
        {
            var books = await _book.GetMainPopularBook();
            return Ok(books);
        }

    }
}