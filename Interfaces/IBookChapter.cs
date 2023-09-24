namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;

public interface IBookChapter
{
    /*    ICollection<BookChapter> GetBookChapters();*/
    Task<ICollection<BookChapter>> GetBookChapterAsync();
}

