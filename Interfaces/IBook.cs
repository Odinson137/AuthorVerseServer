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
    Task<ICollection<BookDTO>> GetSecrtainBooksPage(int page);
    Task<BookDTO?> GetBookById(int bookId);
    Task CreateBook(Book book);
    Task<Genre?> GetGenreById(int id);
    Task AddBookGenre(BookGenre bookGenre);
    Task Save();
    //Task AddBookGenre(Book book, ICollection<int> genresId);
    Task<IDbContextTransaction> BeginTransactionAsync();
    //Task<ICollection<Genre>> GetSelectedGenres(ICollection<int> genresId);
}

