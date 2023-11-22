using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using MailKit.Search;
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
                .Where(book => book.Permission == Data.Enums.PublicationPermission.Approved)
                .OrderByDescending(book => book.Rating)
                .Take(10)
                .Select(book => new PopularBook()
                {
                    BookId = book.BookId,
                    BookCoverUrl = book.BookCover
                });

            return await books.ToListAsync();
        }

        private IQueryable<Book> GetQueryBooksByTagsGenresTitle(int tagId, int genreId, string searchText)
        {
            var query = _context.Books
                .Where(book => book.Permission == Data.Enums.PublicationPermission.Approved);

            if (tagId != 0)
            {
                query = query.Where(book => book.Tags.Any(tag => tag.TagId == tagId));
            }

            if (genreId != 0)
            {
                query = query.Where(book => book.Genres.Any(genre => genre.GenreId == genreId));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(book => book.NormalizedTitle.Contains(searchText));
            }

            return query;
        }

        public async Task<int> GetBooksCountByTagsAndGenres(int tagId, int genreId, string searchText)
        {
            var query = GetQueryBooksByTagsGenresTitle(tagId, genreId, searchText);
            return await query.CountAsync();
        }

        public async Task<ICollection<BookDTO>> GetCertainBooksPage(int tagId, int genreId, int page, string searchText)
        {
            var query = GetQueryBooksByTagsGenresTitle(tagId, genreId, searchText);

            var booksDTO = query
                .AsNoTracking()
                .OrderByDescending(book => book.BookId)
                .Skip(page * 5)
                .Take(5)
                .Select(book => new BookDTO
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Author = new UserDTO
                    {
                        Id = book.AuthorId,
                        UserName = book.Author.UserName,
                    },
                    Genres = book.Genres.Select(genre => new GenreDTO
                    {
                        GenreId = genre.GenreId,
                        Name = genre.Name
                    }).ToList(),
                    Tags = book.Tags.Select(tag => new TagDTO
                    {
                        TagId = tag.TagId,
                        Name = tag.Name
                    }).ToList(),
                    Rating = book.Rating,
                    CountRating = book.CountRating,
                    PublicationData = DateOnly.FromDateTime(book.PublicationData),
                    BookCoverUrl = book.BookCover
                });;

            return await booksDTO.ToListAsync();
        }

        public async Task<DetailBookDTO?> GetBookById(int bookId)
        {
            var book = await _context.Books
                .AsNoTracking()
                .Where(book => book.Permission == Data.Enums.PublicationPermission.Approved &&
                    book.BookId == bookId)
                .Select(book => new DetailBookDTO()
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Description = book.Description,
                    Author = new UserDTO() { Id = book.Author.Id, UserName = book.Author.UserName },
                    Genres = book.Genres.Select(genre => new GenreDTO() { GenreId = genre.GenreId, Name = genre.Name }).ToList(),
                    Tags = book.Genres.Select(tag => new TagDTO() { TagId = tag.GenreId, Name = tag.Name }).ToList(),
                    //ImageUrls = book.BookChapters
                    //        .SelectMany(c => c.ChapterSections.Where(x => !string.IsNullOrEmpty(x.ImageUrl)).Select(x => x.ImageUrl))
                    //        .ToList(),
                    Rating = book.Rating,
                    CountRating = book.CountRating,
                    Choices = book.BookChapters
                            .SelectMany(x => x.ChapterSections
                            .Where(x => x.SectionChoices != null && x.SectionChoices.Count >= 2))
                            .Count(),
                    PublicationData = DateOnly.FromDateTime(book.PublicationData),
                    ChapterCount = book.BookChapters.Count(),
                    BookCoverUrl = book.BookCover
                }).FirstOrDefaultAsync();

            return book;
        }

        public async Task AddBook(Book book)
        {
            await _context.AddAsync(book);
        }

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
                .OrderByDescending(book => book.Rating)
                .Select(book => new MainPopularBook()
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Description = book.Description,
                    Author = new UserDTO { Id = book.AuthorId, UserName = book.Author.UserName },
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
                    Rating = book.Comments.Any() ? book.Comments.Average(x => x.ReaderRating) : 0,
                    Endings = book.BookChapters
                            .SelectMany(x => x.ChapterSections
                            .Where(x => x.NextSectionId == 0))
                            .Count(),
                    Choices = book.BookChapters
                            .SelectMany(x => x.ChapterSections
                            .Where(x => x.SectionChoices != null && x.SectionChoices.Count >= 2))
                            .Count(),
                    BookCoverUrl = book.BookCover,
                    PublicationData = book.PublicationData,
                })
                .Take(5);

            return await books.ToListAsync();
        }

        public async Task<ICollection<AuthorMinimalBook>> GetAuthorBooksAsync(string userId)
        {
            var books = await _context.Books
                .Where(book => book.AuthorId == userId)
                .Select(book => new AuthorMinimalBook() {
                    BookId = book.BookId,
                    Title = book.Title,
                    BookCoverUrl = book.BookCover,
                })
                .ToListAsync();
            return books;
        }

        public Task<ICollection<BookQuotes>> GetBookQuotes()
        {
            throw new NotImplementedException();
        }
    }
}
