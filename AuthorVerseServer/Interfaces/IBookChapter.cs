using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces;
public interface IBookChapter
{
    Task<BookChapter?> GetBookChapterAsync(int commentId, string userId);
    Task<bool> AnyChildExistAsync(int commentId);
    Task<List<BookChapterDTO>> GetBookReadingChaptersAsync(int bookId);
    Task<int> GetUserReadingNumberAsync(int bookId, string userId);
    Task<bool> IsAuthorAsync(int chapterId, string userId);
    Task<ChapterInfo?> GetChapterNumberAsync(int chapterId, string userId);
    Task PublicateChapterAsync(int chapterId);
    Task<List<ShortAuthorChapterDTO>> GetAuthorChaptersAsync(int bookId, string userId);
    Task<DetaildAuthorChapterDTO?> GetAuthorDetaildChapterAsync(int chapter, string userId);
    Task AddNewChapterAsync(BookChapter chapter);
    Task DeleteChapterAsync(int chapterId);
    Task<NotifyChapter> GetNotifyChapterAsync(int chapterId);
    Task SaveAsync();
}

