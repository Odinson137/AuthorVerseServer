namespace AuthorVerseServer.Interfaces;

using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

public interface IBookChapter
{
    /*    ICollection<BookChapter> GetBookChapters();*/
    Task<ICollection<BookChapterDTO>> GetBookChapterAsync();
}

