using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBook _book;
        private readonly UserManager<User> _userManager;
        private readonly IMemoryCache _cache;

        public BookController(IBook book, UserManager<User> userManager, IMemoryCache cache)
        {
            _book = book;
            _userManager = userManager;
            _cache = cache;
        }

        [HttpGet("Popular")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<PopularBook>>> GetPopularBooks()
        {
            var books = await _cache.GetOrCreateAsync("popularMainBooks", async entry =>
            {
                var bookDb = await _book.GetPopularBooks();
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return bookDb;
            });
            return Ok(books);
        }

        [HttpGet("Last")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<PopularBook>>> GetLastBooks()
        {
            var books = await _cache.GetOrCreateAsync("lastMainBooks", async entry =>
            {
                var bookDb = await _book.GetLastBooks();
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return bookDb;
            });

            return Ok(books);
        }

        [HttpGet("SearchBy")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<BookPageDTO>> GetCertainBooksPage(int tagId = 0, int genreId = 0, int page = 0)
        {
            var books = await _book.GetCertainBooksPage(tagId, genreId, page);

            int count = page == 0 ? await _book.GetBooksCountByTagsAndGenres(tagId, genreId) : 0;
            
            return Ok(new BookPageDTO
            {
                Books = books,
                BooksCount = count
            });
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
            var books = await _cache.GetOrCreateAsync("mainPopularBooks", async entry =>
            {
                var bookDb = await _book.GetMainPopularBook();
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return bookDb;
            });
            return Ok(books);
        }

    }
}