namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;

public interface IBook
{
    Task<ICollection<Book>> GetBookAsync();
}

