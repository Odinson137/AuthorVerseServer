using AsyncAwaitBestPractices;
using AuthorVerseServer.Data.ControllerSettings;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : AuthorVerseController
    {
        private readonly IBook _book;
        private readonly IDatabase _redis;
        private readonly LoadFileService _loadImage;

        public BookController(IBook book, IConnectionMultiplexer redisConnection, LoadFileService loadImage)
        {
            _book = book;
            _redis = redisConnection.GetDatabase();
            _loadImage = loadImage;
        }

        [HttpGet("Count")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<int>> GetBooksCount()
        {
            if (int.TryParse(await _redis.StringGetAsync("booksCount"), out var cachedValue))
            {
                return Ok(cachedValue);
            }

            int countDb = await _book.GetCountBooks();

            await _redis.StringSetAsync("booksCount", countDb, TimeSpan.FromHours(1));
            return Ok(countDb);
        }

        [HttpGet("Popular")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<PopularBook>>> GetPopularBooks()
        {
            var booksCache = await _redis.StringGetAsync("popularMainBooks");

            ICollection<PopularBook>? books;
            if (string.IsNullOrEmpty(booksCache))
            {
                books = await _book.GetPopularBooks();
                await _redis.StringSetAsync(
                    "popularMainBooks", 
                    JsonConvert.SerializeObject(books), 
                    TimeSpan.FromHours(1));
            } else
            {
                books = JsonConvert.DeserializeObject<ICollection<PopularBook>>(booksCache);
            }

            return Ok(books);
        }

        [HttpGet("Last")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<PopularBook>>> GetLastBooks()
        {
            var booksCache = await _redis.StringGetAsync("lastMainBooks");

            ICollection<PopularBook>? books;
            if (string.IsNullOrEmpty(booksCache))
            {
                books = await _book.GetLastBooks();
                await _redis.StringSetAsync(
                    "lastMainBooks", 
                    JsonConvert.SerializeObject(books), 
                    TimeSpan.FromHours(1));
            }
            else
            {
                books = JsonConvert.DeserializeObject<ICollection<PopularBook>>(booksCache!);
            }

            return Ok(books);
        }

        [HttpGet("SearchBy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BookPageDTO>> GetCertainBooksPage(
            int tagId = 0, int genreId = 0, int page = 1, string searchText = "")
        {
            _redis.HashIncrementAsync($"searcher",
                    $"tagId-{tagId}:genreId-{genreId}", 1, CommandFlags.FireAndForget)
                .SafeFireAndForget();
            
            if (--page < 0)
                return BadRequest("Page is smaller than zero");

            var books = await _book.GetCertainBooksPage(tagId, genreId, page, searchText);

            int count = page == 0 ? await _book.GetBooksCountByTagsAndGenres(tagId, genreId, searchText) : 0;
            
            return Ok(new BookPageDTO
            {
                Books = books,
                BooksCount = count
            });
        }

        [Authorize]
        [HttpPost("Create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<int>> CreateBook([FromBody] CreateBookDTO bookDTO)
        {
            if (bookDTO.GenresId.Count == 0)
            {
                return BadRequest("Genres are not select");
            }
   
            if (bookDTO.TagsId.Count == 0)
                return BadRequest("Tags are not select");

            Book book = new Book()
            {
                AuthorId = UserId,
                Title = bookDTO.Title,
                Description = bookDTO.Description,
                AgeRating = bookDTO.AgeRating,
            };

            if (bookDTO.BookCoverImage != null)
            {
                string nameFile = _loadImage.GetUniqueName(bookDTO.BookCoverImage);
                await _loadImage.CreateFileAsync(bookDTO.BookCoverImage, nameFile, "images");
                book.BookCover = nameFile;
            }

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

            await _book.AddBook(book);

            await _book.SaveAsync();

            return Ok(book.BookId);
        }

        [HttpGet("MainPopularBooks")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<MainPopularBook>>> GetMainPopularBooks()
        {
            var booksCache = await _redis.StringGetAsync("mainPopularBooks");

            ICollection<MainPopularBook>? books;
            if (string.IsNullOrEmpty(booksCache))
            {
                books = await _book.GetMainPopularBook();
                await _redis.StringSetAsync(
                    "mainPopularBooks", 
                    JsonConvert.SerializeObject(books), 
                    TimeSpan.FromHours(1));
            }
            else
            {
                books = JsonConvert.DeserializeObject<ICollection<MainPopularBook>>(booksCache);
            }

            return Ok(books);
        }


        [HttpGet("Short/{bookId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ShoptBookDTO>> GetShortBook(int bookId)
        {
            var book = await _book.GetShortBookById(bookId);

            if (book == null)
            {
                return NotFound("This book not found");
            }

            return book;
        }

        [HttpGet("Detail/{bookId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DetailBookDTO>> GetDetailBook(int bookId)
        {
            _redis.HashIncrementAsync($"bookPage",
                    $"bookId-{bookId}", 1, CommandFlags.FireAndForget)
                .SafeFireAndForget();
            
            var book = await _book.GetBookById(bookId);

            if (book == null)
            {
                return NotFound("This book not found");
            }

            return book;
        }


        [HttpGet("AuthorBooks")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<MinimalBook>>> GetAuthorBooks(string authorId)
        {
            var books = await _book.GetAuthorBooksAsync(authorId);
            return Ok(books);
        }

        [HttpGet("SimilarBooks")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<MinimalBook>>> GetSimilarBook(int bookId)
        {
            var book = await _book.GetBookGenresTagsAsync(bookId);
            if (book == null)
            {
                return NotFound("Book not found");
            }

            var books = await _book.GetSimilarBooksAsync(bookId, book);
            return Ok(books);
        }
    }
}