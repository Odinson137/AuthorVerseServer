namespace AuthorVerseServer.Interfaces;

using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore.Storage;

public interface IBook
{
    Task<ICollection<Book>> GetBooksAsync();
    Task<int> GetCountBooks();
    Task<ICollection<PopularBook>> GetPopularBooks();
    Task<ICollection<PopularBook>> GetLastBooks();
    Task<ICollection<BookDTO>> GetCertainBooksPage(int page);
    Task<ICollection<MainPopularBook>> GetMainPopularBook();
    Task<BookDTO?> GetBookById(int bookId);
    Task AddBook(Book book);
    Task<Genre?> GetGenreById(int id);
    Task<Tag?> GetTagById(int id);
    Task Save();
}

