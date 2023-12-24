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

        public Task<List<Book>> GetBooksAsync()
        {
            return _context.Books.AsNoTracking().OrderBy(g => g.BookId).ToListAsync();
        }

        public Task<int> GetCountBooks()
        {
            return _context.Books.CountAsync();
        }

        public Task<List<PopularBook>> GetLastBooks()
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

            return books.ToListAsync();
        }

        public Task<List<PopularBook>> GetPopularBooks()
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

            return books.ToListAsync();
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

        public Task<int> GetBooksCountByTagsAndGenres(int tagId, int genreId, string searchText)
        {
            var query = GetQueryBooksByTagsGenresTitle(tagId, genreId, searchText);
            return query.CountAsync();
        }

        public Task<List<BookDTO>> GetCertainBooksPage(int tagId, int genreId, int page, string searchText)
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

            return booksDTO.ToListAsync();
        }

        public Task<ShoptBookDTO?> GetShortBookById(int bookId)
        {
            var book = _context.Books
                .AsNoTracking()
                .Where(book => book.Permission == Data.Enums.PublicationPermission.Approved)
                .Where(book => book.BookId == bookId)
                .Select(book => new ShoptBookDTO()
                {
                    Title = book.Title,
                    AuthorName = book.Author.UserName,
                }).FirstOrDefaultAsync();

            return book;
        }

        public Task<DetailBookDTO?> GetBookById(int bookId)
        {
            var book = _context.Books
                .AsNoTracking()
                .Where(book => book.Permission == Data.Enums.PublicationPermission.Approved &&
                    book.BookId == bookId)
                .Select(book => new DetailBookDTO()
                {
                    Title = book.Title,
                    Description = book.Description,
                    Author = new UserDTO() { Id = book.Author.Id, UserName = book.Author.UserName },
                    Genres = book.Genres.Select(genre => new GenreDTO() { GenreId = genre.GenreId, Name = genre.Name }).ToList(),
                    Tags = book.Genres.Select(tag => new TagDTO() { TagId = tag.GenreId, Name = tag.Name }).ToList(),

                    Rating = book.Rating,
                    CountRating = book.CountRating,
                    Choices = book.BookChapters
                            .SelectMany(x => x.ChapterSections
                            .Where(x => x.SectionChoices != null && x.SectionChoices.Count >= 2)
                            )
                            .Count(),
                    PublicationData = DateOnly.FromDateTime(book.PublicationData),
                    ChapterCount = book.BookChapters.Count(),
                    BookCoverUrl = book.BookCover
                }).FirstOrDefaultAsync();

            return book;
        }

        public Task AddBook(Book book)
        {
            _context.AddAsync(book);
            return Task.CompletedTask;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public ValueTask<Genre?> GetGenreById(int id)
        {
            return _context.Genres.FindAsync(id);
        }

        public ValueTask<Tag?> GetTagById(int id)
        {
            return _context.Tags.FindAsync(id);
        }

        public Task<List<MainPopularBook>> GetMainPopularBook()
        {
            var books = _context.Books
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
                            .SelectMany(x => x.ChapterSections)
                            .Select(x => x.ChoiceFlow)
                            .Distinct()
                            .Count(),
                    Choices = book.BookChapters
                            .SelectMany(x => x.ChapterSections
                            .Where(x => x.SectionChoices != null && x.SectionChoices.Count >= 2)
                            )
                            .Count(),
                    BookCoverUrl = book.BookCover,
                    PublicationData = book.PublicationData,
                })
                .Take(5);

            return books.ToListAsync();
        }

        public async Task<List<MinimalBook>> GetAuthorBooksAsync(string userId)
        {
            var books = await _context.Books
                .AsNoTracking()
                .Where(book => book.AuthorId == userId)
                .Select(book => new MinimalBook() {
                    BookId = book.BookId,
                    Title = book.Title,
                    BookCoverUrl = book.BookCover,
                })
                .ToListAsync();
            return books;
        }


        public async Task<List<MinimalBook>> GetSimilarBooksAsync(int bookId, GenreTagDTO currentBook)
        {
            var request = _context.Books
                .AsNoTracking()
                .Where(book => book.BookId != bookId)
                .Where(book => book.Genres.Any(g => currentBook.Genres.Contains(g.GenreId)))
                .Where(book => book.Tags.Any(t => currentBook.Tags.Contains(t.TagId)))
                .OrderByDescending(book =>
                    book.Genres.Count(g => currentBook.Genres.Contains(g.GenreId)) +
                    book.Tags.Count(t => currentBook.Tags.Contains(t.TagId)))
                .Take(10)
                .Select(book => new MinimalBook
                {
                    BookId = book.BookId,
                    BookCoverUrl = book.BookCover,
                    Title = book.Title
                });

            return await request.ToListAsync();
        }


        public async Task<bool> ExistBookAsync(int bookId)
        {
            return await _context.Books.AnyAsync(book => book.BookId == bookId);
        }

        public async Task<GenreTagDTO?> GetBookGenresTagsAsync(int bookId)
        {
            return await _context.Books
                .AsNoTracking()
                .Where(book => book.BookId == bookId)
                .Select(book => new GenreTagDTO
                {
                    Genres = book.Genres.Select(genre => genre.GenreId).ToList(),
                    Tags = book.Tags.Select(tag => tag.TagId).ToList(),
                }).FirstOrDefaultAsync();
        }


    }
}
