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
            var books = await _context.Books.Select(book => new BookDTO()
            { 
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                Author = book.Author,
                BookGenres = book.BookGenres,
                AgeRating = book.AgeRating
            }).ToListAsync();

            return books;
        }
    }
}
