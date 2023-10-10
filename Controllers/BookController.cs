using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Enums;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Http;
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
        public DataContext _context { get; set; }

        public BookController(DataContext context, IBook book)
        {
            _context = context;
            _book = book;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<Book>>> GetBooks()
        {
            var books = await _book.GetBookAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(books);
        }

        [HttpGet("Count")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<int>> GetCountBooks()
        {
            int bookCount = await _context.Books.CountAsync();
            return bookCount;
        }

        [HttpGet("Popular")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<PopularBook>>> GetPopularBooks()
        {
            var books = await _context.Books
                .AsNoTracking()
                .OrderByDescending(book => book.AverageRating)
                .Take(10)
                .Select(book => new PopularBook()
                {
                    BookId = book.BookId,
                    BookCoverUrl = book.BookCover
                }).ToListAsync();

            return books;
        }

        [HttpGet("Last")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<PopularBook>>> GetLastBooks()
        {
            var books = await _context.Books
                .AsNoTracking()
                .OrderByDescending(book => book.PublicationData)
                .Take(10)
                .Select(book => new PopularBook()
                {
                    BookId = book.BookId,
                    BookCoverUrl = book.BookCover ?? ""
                }).ToListAsync();

            return books;
        }

        [HttpGet("Page/{intPage?}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<BookDTO>>> GetLastBooks(int intPage = 0)
        {
            var books = await _context.Books
                .AsNoTracking()
                .Skip(intPage * 5)
                .Take(5)
                .Select(book => new BookDTO()
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Author = new UserDTO()
                    {
                        Id = book.AuthorId,
                        UserName = book.Author.UserName ?? "No name",
                    },
                    Genres = book.Genres.Select(genre => new GenreDTO()
                    {
                        GenreId = genre.GenreId,
                        Name = genre.Name
                    }).ToList(),
                    AgeRating = book.AgeRating,
                    BookCoverUrl = book.BookCover
                }).ToListAsync();

            return books;
        }

        [HttpGet("{bookId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BookDTO>> GetBook(int bookId)
        {
            var book = await _context.Books.AsNoTracking().Include(book => book.Genres).Select(book => new BookDTO()
            {
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                Author = new UserDTO() { Id = book.Author.Id, UserName = book.Author.UserName },
                Genres = book.Genres.Select(genre => new GenreDTO() { GenreId = genre.GenreId, Name = genre.Name }).ToList(),
                AgeRating = book.AgeRating
            }).Where(book => book.BookId == bookId).FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound("This book not found");
            }

            return book;
        }
    }
}