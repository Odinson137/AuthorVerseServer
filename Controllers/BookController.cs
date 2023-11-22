using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBook _book;
        private readonly UserManager<User> _userManager;
        private readonly IDatabase _cache;
        private readonly ILoadImage _loadImage;

        public BookController(IBook book, UserManager<User> userManager, IConnectionMultiplexer redisConnection, ILoadImage loadImage)
        {
            _book = book;
            _userManager = userManager;
            _cache = redisConnection.GetDatabase();
            _loadImage = loadImage;
        }

        [HttpGet("Count")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<int>> GetBooksCount()
        {
            if (int.TryParse(await _cache.StringGetAsync("booksCount"), out var cachedValue))
            {
                return Ok(cachedValue);
            }

            int countDb = await _book.GetCountBooks();

            await _cache.StringSetAsync("booksCount", countDb, TimeSpan.FromHours(1));
            return Ok(countDb);
        }

        [HttpGet("Popular")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<PopularBook>>> GetPopularBooks()
        {
            var booksCache = await _cache.StringGetAsync("popularMainBooks");

            ICollection<PopularBook>? books;
            if (string.IsNullOrEmpty(booksCache))
            {
                books = await _book.GetPopularBooks();
                await _cache.StringSetAsync(
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
            var booksCache = await _cache.StringGetAsync("lastMainBooks");

            ICollection<PopularBook>? books;
            if (string.IsNullOrEmpty(booksCache))
            {
                books = await _book.GetLastBooks();
                await _cache.StringSetAsync(
                    "lastMainBooks", 
                    JsonConvert.SerializeObject(books), 
                    TimeSpan.FromHours(1));
            }
            else
            {
                books = JsonConvert.DeserializeObject<ICollection<PopularBook>>(booksCache);
            }

            return Ok(books);
        }

        [HttpGet("SearchBy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BookPageDTO>> GetCertainBooksPage(
            int tagId = 0, int genreId = 0, int page = 1, string searchText = "")
        {
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
                NormalizedTitle = bookDTO.Title.ToUpper(),
                Description = bookDTO.Description,
                AgeRating = bookDTO.AgeRating,
            };

            if (bookDTO.BookCoverImage != null)
            {
                string nameFile = _loadImage.GetUniqueName(bookDTO.BookCoverImage);
                await _loadImage.CreateImageAsync(bookDTO.BookCoverImage, nameFile, "Images");
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

            try
            {
                await _book.Save();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return Ok(book.BookId);
        }

        [HttpGet("MainPopularBooks")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<MainPopularBook>>> GetMainPopularBooks()
        {
            var booksCache = await _cache.StringGetAsync("mainPopularBooks");

            ICollection<MainPopularBook>? books;
            if (string.IsNullOrEmpty(booksCache))
            {
                books = await _book.GetMainPopularBook();
                await _cache.StringSetAsync(
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


        [HttpGet("{bookId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<DetailBookDTO>> GetBook(int bookId)
        {
            var book = await _book.GetBookById(bookId);

            if (book == null)
            {
                return NotFound("This book not found");
            }

            return book;
        }


        [HttpGet("AuthorBooks")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<AuthorMinimalBook>>> GetAuthorBooks(string authorId)
        {
            var books = await _book.GetAuthorBooksAsync(authorId);
            return Ok(books);
        }

        [HttpGet("BookQuotes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<QuoteDTO>>> GetBookQuotes(int bookId, int page = 1)
        {
            if (--page < 0)
            {
                return BadRequest("Error page");
            }
            var quotes = await _book.GetBookQuotesAsync(bookId, page);
            return Ok(quotes);
        }
    }
}