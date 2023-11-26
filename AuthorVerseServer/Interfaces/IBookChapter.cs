namespace AuthorVerseServer.Interfaces;

using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

public interface IBookChapter
{
    Task<ICollection<BookChapterDTO>> GetBookChapterAsync();
}

