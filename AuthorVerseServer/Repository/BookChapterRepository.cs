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

        public async Task<ICollection<ShortAuthorChapterDTO>> GetAuthorChaptersAsync(int bookId, string userId)
        {
            var chapters = _context.BookChapters
                .Where(c => c.BookId == bookId)
                .Where(c => c.Book.AuthorId == userId)
                .Select(c => new ShortAuthorChapterDTO
                {
                    BookChapterId = c.BookChapterId,
                    Title = c.Title,
                    Number = c.BookChapterNumber,
                    Place = c.ActionPlace,
                    CharacterCount = c.Characters.Count(),
                });

            return await chapters.ToListAsync();
        }

        public async Task<DetaildAuthorChapterDTO?> GetAuthorDetaildChapterAsync(int chapterId, string userId)
        {
            var chapter = await _context.BookChapters
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.Book.AuthorId == userId)
                .Select(c => new DetaildAuthorChapterDTO()
                {
                    Characters = c.Characters.Select(character => new CharacterDTO
                    {
                        CharacterId = character.CharacterId,
                        Name = character.Name,
                    }).ToList(),
                    Description = c.Description,
                })
                .FirstOrDefaultAsync();

            return chapter;
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

        public async Task<BookChapter?> GetBookChapterAsync(int commentId, string userId)
        {
            return await _context.BookChapters
                .Where(c => c.BookChapterId == commentId)
                .Where(c => c.Book.AuthorId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<NotifyChapter> GetNotifyChapterAsync(int chapterId)
        {
            var chapter = _context.BookChapters
                .Where(c => c.BookChapterId == chapterId)
                .Select(c => new NotifyChapter
                {
                    BookId = c.BookId,
                    BookTitle = c.Book.Title,
                    ChapterNumber = c.BookChapterNumber,
                    ChapterTitle = c.Title,
                    Url = c.Book.BookCover,
                });

            return await chapter.FirstAsync();
        }
    }
}

