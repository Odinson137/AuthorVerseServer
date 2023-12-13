using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces;
public interface IBookChapter
{
    Task<ICollection<BookChapterDTO>> GetBookReadingChaptersAsync(int bookId);
    Task<int> GetUserReadingNumberAsync(int bookId, string userId);
    Task<bool> IsAuthorAsync(int bookId, string userId);
    Task<(int, int)?> GetChapterNumberAsync(int chapterId, string userId);
    Task PublicateChapterAsync(int chapterId);
    Task<ICollection<ShortAuthorChapterDTO>> GetAuthorChaptersAsync(int bookId);
    Task<DetaildAuthorChapterDTO> GetAuthorDetaildChapterAsync(int chapter);
    Task AddNewChapterAsync(BookChapter chapter);
    Task DeleteChapterAsync(int chapterId);
    Task SaveAsync();
}

