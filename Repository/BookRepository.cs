using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthorVerseServer.Repository
{
    public class BookRepository : IBook
    {
        private readonly DataContext _context;
        public BookRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Book>> GetBooksAsync()
        {
            return await _context.Books.AsNoTracking().OrderBy(g => g.BookId).ToListAsync();
        }

        public async Task<int> GetCountBooks()
        {
            return await _context.Books.CountAsync();
        }

        public async Task<ICollection<PopularBook>> GetLastBooks()
        {
            var books = _context.Books
                .AsNoTracking()
                .OrderByDescending(book => book.PublicationData)
                .Take(10)
                .Select(book => new PopularBook()
                {
                    BookId = book.BookId,
                    BookCoverUrl = book.BookCover ?? ""
                });

            return await books.ToListAsync();
        }

        public async Task<ICollection<PopularBook>> GetPopularBooks()
        {
            var books = _context.Books
                .AsNoTracking()
                .OrderByDescending(book => book.AverageRating)
                .Take(10)
                .Select(book => new PopularBook()
                {
                    BookId = book.BookId,
                    BookCoverUrl = book.BookCover
                });

            return await books.ToListAsync();
        }

        public async Task<ICollection<BookDTO>> GetSecrtainBooksPage(int page)
        {
            var books = _context.Books
            .AsNoTracking()
            .Skip(page * 5)
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
            });

            return await books.ToListAsync();
        }

        public async Task<BookDTO?> GetBookById(int bookId)
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

        public async Task CreateBook(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Genre?> GetGenreById(int id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task AddBookGenre(BookGenre bookGenre)
        {
            await _context.BookGenres.AddAsync(bookGenre);
        }
    }
}
