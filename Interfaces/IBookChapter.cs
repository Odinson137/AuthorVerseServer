namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;

public interface IBookChapter
{
    Task<ICollection<BookChapter>> GetBookChapterAsync();
}

