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

        [HttpGet("Books")]
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

        [HttpGet("Books/Count")]
        public async Task<int> GetCountBooks()
        {
            int bookCount = await _context.Books.CountAsync();
            return bookCount;
        }

        [HttpGet("Books/Popular")]
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

        [HttpGet("Books/Last")]
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

        [HttpGet("Genres/{bookId}")]
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
            }).Where(book => book.BookId == bookId).FirstOrDefaultAsync() ?? new BookDTO { Title = "Error" };

            return book;
        }

        [HttpGet("Comments/{bookId}")]
        public async Task<ICollection<CommentDTO>> GetBookComments(int bookId)
        {
            var comments = await _context.Comments.AsNoTracking()
                .Include(comment => comment.Commentator)
                .Where(comments => comments.BookId == bookId)
                .Select(comment => new CommentDTO()
                {
                    CommentId = comment.CommentId,
                    Commentator = new UserDTO { Id = comment.Commentator.Id, UserName = comment.Commentator.UserName }
                })
            .ToListAsync();

            return comments;
        }

        [HttpGet("Genres")]
        public async Task<ICollection<GenreDTO>> GetGenre()
        {
            var genres = await _context.Genres.AsNoTracking().Select(genre => new GenreDTO()
            {
                GenreId = genre.GenreId,
                Name = genre.Name
            }).ToListAsync();

            return genres;
        }
    }
}