using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class BookChapterRepository: IBookChapter
    {
        private readonly DataContext _context;
        public BookChapterRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddNewChapterAsync(BookChapter chapter)
        {
            await _context.BookChapters.AddAsync(chapter);
        }

        public async Task DeleteChapterAsync(int chapterId)
        {
            await _context.BookChapters.Where(c => c.BookChapterId == chapterId).ExecuteDeleteAsync();
        }

        public Task<ICollection<ShortAuthorChapterDTO>> GetAuthorChaptersAsync(int bookId)
        {
            throw new NotImplementedException();
        }

        public Task<DetaildAuthorChapterDTO> GetAuthorDetaildChapterAsync(int chapter)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetUserReadingNumberAsync(int bookId, string userId)
        {
            var number = await _context.UserSelectedBooks
                .Where(s => s.BookId == bookId)
                .Where(s => s.UserId == userId)
                .Select(s => s.LastBookChapterNumber)
                .FirstOrDefaultAsync();

            return number;
        }

        public async Task<ICollection<BookChapterDTO>> GetBookReadingChaptersAsync(int bookId)
        {
            var chapters = await _context.BookChapters
                .Where(c => c.BookId == bookId)
                .Select(c => new BookChapterDTO
                {
                    BookChapterId = c.BookChapterId,
                    Number = c.BookChapterNumber,
                    Title = c.Title,
                }).ToListAsync();

            return chapters;
        }

        public async Task<bool> IsAuthorAsync(int bookId, string userId)
        {
            return await _context.Books
                .Where(b => b.BookId == bookId)
                .Where(b => b.AuthorId == userId)
                .AnyAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<(int, int)?> GetChapterNumberAsync(int chapterId, string userId)
        {
            var result = await _context.BookChapters
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.Book.AuthorId == userId)
                .Select(c => new { c.BookChapterNumber, c.BookId })
                .FirstOrDefaultAsync();

            if (result == null) return null;
            return (result.BookChapterNumber, result.BookId);
        }

        public async Task PublicateChapterAsync(int chapterId)
        {
            await _context.BookChapters
                .Where(c => c.BookChapterId == chapterId)
                .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.PublicationType, Data.Enums.PublicationType.Publicated));
        }
    }
}

