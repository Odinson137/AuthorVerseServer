using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Enums;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

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
            return Ok(await _book.GetSecrtainBooksPage(page));
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

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> CreateBook([FromBody] CreateBookDTO bookDTO)
        {
            if (string.IsNullOrEmpty(bookDTO.AuthorId))
            {
                return BadRequest("Invalid user Id");
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

            await _book.CreateBook(book);

            if (bookDTO.GenresId != null)
            {
                foreach (var genreId in bookDTO.GenresId)
                {
                    var genre = await _book.GetGenreById(genreId);

                    if (genre != null)
                    {
                        var bookGenre = new BookGenre
                        {
                            Book = book,
                            Genre = genre
                        };

                        await _book.AddBookGenre(bookGenre);
                    }
                }
            }

            await _book.Save();

            return Ok("Book created");
        }
    }
}