using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

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
                .Where(book => book.Permission == Data.Enums.PublicationPermission.Approved)
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
                .Include(book => book.Ratings)
                .Where(book => book.Permission == Data.Enums.PublicationPermission.Approved)
                .OrderByDescending(book => book.Ratings.Max(x => x.Rating))
                .Take(10)
                .Select(book => new PopularBook()
                {
                    BookId = book.BookId,
                    BookCoverUrl = book.BookCover
                });

            return await books.ToListAsync();
        }

        public async Task<ICollection<BookDTO>> GetCertainBooksPage(int page)
        {
            var books = _context.Books
            .AsNoTracking()
            .Where(book => book.Permission == Data.Enums.PublicationPermission.Approved)
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
                Tags = book.Tags.Select(tag => new TagDTO()
                {
                    TagId = tag.TagId,
                    Name = tag.Name
                }).ToList(),
                AgeRating = book.AgeRating,
                BookCoverUrl = book.BookCover
            });

            return await books.ToListAsync();
        }

        public async Task<BookDTO?> GetBookById(int bookId)
        {
            var book = await _context.Books
                .AsNoTracking()
                .Where(book => book.Permission == Data.Enums.PublicationPermission.Approved &&
                    book.BookId == bookId)
                .Select(book => new BookDTO()
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Description = book.Description,
                    Author = new UserDTO() { Id = book.Author.Id, UserName = book.Author.UserName },
                    Genres = book.Genres.Select(genre => new GenreDTO() { GenreId = genre.GenreId, Name = genre.Name }).ToList(),
                    Tags = book.Genres.Select(tag => new TagDTO() { TagId = tag.GenreId, Name = tag.Name }).ToList(),
                    AgeRating = book.AgeRating
                }).FirstOrDefaultAsync();

            return book;
        }

        public async Task AddBook(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        //public async Task<IDbContextTransaction> BeginTransactionAsync()
        //{
        //    return await _context.Database.BeginTransactionAsync();
        //}

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Genre?> GetGenreById(int id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task<Tag?> GetTagById(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task<ICollection<MainPopularBook>> GetMainPopularBook()
        {
            var books = _context.Books
                .AsNoTracking()
                .Where(book => book.Permission == Data.Enums.PublicationPermission.Approved)
                .OrderByDescending(book => book.Ratings.Max(x => x.Rating))
                .Select(book => new MainPopularBook()
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Description = book.Description,
                    AuthorId = book.AuthorId,
                    AuthorUserName = book.Author.UserName,
                    Genres = book.Genres.Select(genre => new GenreDTO
                    {
                        GenreId = genre.GenreId,
                        Name = genre.Name,
                    }).ToList(),
                    Tags = book.Tags.Select(tag => new TagDTO
                    {
                        TagId = tag.TagId,
                        Name = tag.Name,
                    }).ToList(),
                    Rating = book.Ratings.Any() ? book.Ratings.Average(x => x.Rating) : 0,
                    Endings = book.BookChapters
                            .SelectMany(x => x.ChapterSections
                            .Where(x => x.NextSectionId == 0))
                            .Count(),
                    Choices = book.BookChapters
                            .SelectMany(x => x.ChapterSections
                            .Where(x => x.SectionChoices != null && x.SectionChoices.Count >= 2))
                            .Count(),
                    BookCoverUrl = book.BookCover,
                    BookPanoramUrl = book.BookPanoramaImage,
                    PublicationData = book.PublicationData,
                })
                .Take(5);

            return await books.ToListAsync();
        }
    }
}
