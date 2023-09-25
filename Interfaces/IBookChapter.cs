namespace AuthorVerseServer.Interfaces;

using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

public interface IBookChapter
{
<<<<<<< HEAD
    /*    ICollection<BookChapter> GetBookChapters();*/
    Task<ICollection<BookChapterDTO>> GetBookChapterAsync();
=======
    Task<ICollection<BookChapter>> GetBookChapterAsync();
>>>>>>> 98aa98ac915488b23ad69ea3a3d06058f80da620
}

