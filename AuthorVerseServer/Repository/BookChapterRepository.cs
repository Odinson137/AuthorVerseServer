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

        public Task AddNewChapterAsync(BookChapter chapter)
        {
            _context.BookChapters.AddAsync(chapter);
            return Task.CompletedTask;
        }

        public Task DeleteChapterAsync(int chapterId)
        {
            _context.BookChapters
                .Where(c => c.BookChapterId == chapterId)
                .ExecuteDeleteAsync();
            return Task.CompletedTask;
        }

        public Task<List<ShortAuthorChapterDTO>> GetAuthorChaptersAsync(int bookId, string userId)
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

            return chapters.ToListAsync();
        }

        public Task<DetaildAuthorChapterDTO?> GetAuthorDetaildChapterAsync(int chapterId, string userId)
        {
            var chapter = _context.BookChapters
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

        public Task<int> GetUserReadingNumberAsync(int bookId, string userId)
        {
            var number = _context.UserSelectedBooks
                .Where(s => s.BookId == bookId)
                .Where(s => s.UserId == userId)
                .Select(s => s.LastBookChapterNumber)
                .FirstOrDefaultAsync();

            return number;
        }

        public  Task<List<BookChapterDTO>> GetBookReadingChaptersAsync(int bookId)
        {
            var chapters = _context.BookChapters
                .Where(c => c.BookId == bookId)
                .Select(c => new BookChapterDTO
                {
                    BookChapterId = c.BookChapterId,
                    Number = c.BookChapterNumber,
                    Title = c.Title,
                }).ToListAsync();

            return chapters;
        }
        
        public  Task<bool> IsAuthorAsync(int chapterId, string userId)
        {
            return _context.BookChapters
                .Where(b => b.BookChapterId == chapterId)
                .Where(b => b.Book.AuthorId == userId)
                .AnyAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public  Task<ChapterInfo?> GetChapterNumberAsync(int chapterId, string userId)
        {
            var result = _context.BookChapters
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.Book.AuthorId == userId)
                .Select(c => new ChapterInfo 
                { ChapterNumber = c.BookChapterNumber, 
                    BookId = c.BookId 
                })
                .FirstOrDefaultAsync();

            return result;
        }

        public  Task PublicateChapterAsync(int chapterId)
        {
            _context.BookChapters
                .Where(c => c.BookChapterId == chapterId)
                .ExecuteUpdateAsync(setter =>
                    setter.SetProperty(x => x.PublicationType, Data.Enums.PublicationType.Publicated));
            return Task.CompletedTask;
        }

        public Task<BookChapter?> GetBookChapterAsync(int commentId, string userId)
        {
            return _context.BookChapters
                .Where(c => c.BookChapterId == commentId)
                .Where(c => c.Book.AuthorId == userId)
                .FirstOrDefaultAsync();
        }

        public Task<NotifyChapter> GetNotifyChapterAsync(int chapterId)
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

            return chapter.FirstAsync();
        }

        public Task<bool> AnyChildExistAsync(int commentId)
        {
            var hasChild = _context.BookChapters
                .Where(c => c.BookChapterId == commentId)
                .SelectMany(c => c.Book.BookChapters)
                .Where(c => c.BookChapterId > commentId)
                .AnyAsync();

            return hasChild;
        }
    }
}

