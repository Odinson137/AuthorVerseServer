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

        //[HttpGet("Genres/{bookId}")]
        //public async Task<ICollection<CommentDTO>> GetBookComments(int bookId)
        //{
        //    var comments = await _context.Comments.AsNoTracking()
        //        .Include(comment => comment.User)
        //        .Where(comments => comments.BookId == bookId)
        //        .Select(comment => new CommentDTO()
        //        {
        //            CommentId = comment.CommentId,
        //            Commentator = new UserDTO { Id = comment.User.Id, UserName = comment.User.UserName }
        //        })
        //    .ToListAsync();

        //    return comments;
        //}

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