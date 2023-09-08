using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Enums;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        public DataContext _context { get; set; }

        public BookController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ICollection<BookDTO>> GetBooks()
        {
            var books = await _context.Books.AsNoTracking().Include(book => book.Genres).Select(book => new BookDTO()
            {
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                Author = new UserDTO() { Id = book.Author.Id, UserName = book.Author.UserName },
                Genres = book.Genres.Select(genre => new GenreDTO() { GenreId = genre.GenreId, Name = genre.Name }).ToList(),
                AgeRating = book.AgeRating
            }).ToListAsync();

            return books;
        }

        [HttpGet("Count")]
        public async Task<int> GetCountBooks()
        {
            int bookCount = await _context.Books.CountAsync();
            return bookCount;
        }

        [HttpGet("Popular")]
        public async Task<ICollection<PopularBook>> GetPopularBooks()
        {
            var books = await _context.Books
                .AsNoTracking()
                .OrderByDescending(book => book.AverageRating)
                .Take(10)
                .Select(book => new PopularBook()
                {
                    BookId = book.BookId,
                    BookCover = book.BookCover
                }).ToListAsync();

            return books;
        }

        [HttpGet("Last")]
        public async Task<ICollection<PopularBook>> GetLastBooks()
        {
            var books = await _context.Books
                .AsNoTracking()
                .OrderByDescending(book => book.PublicationData)
                .Take(10)
                .Select(book => new PopularBook()
                {
                    BookId = book.BookId,
                    BookCover = book.BookCover
                }).ToListAsync();

            return books;
        }

        [HttpGet("Page/{intPage?}")]
        public async Task<ICollection<BookDTO>> GetLastBooks(int intPage = 0)
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
                        UserName = book.Author.UserName,
                    },
                    Genres = book.Genres.Select(genre => new GenreDTO()
                    {
                        GenreId = genre.GenreId,
                        Name = genre.Name
                    }).ToList(),
                    AgeRating = book.AgeRating,
                    BookCover = book.BookCover
                }).ToListAsync();

            return books;
        }

        [HttpGet("{bookId}")]
        public async Task<BookDTO> GetBook(int bookId)
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

            return book;
        }
    }
}