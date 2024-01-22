using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using MailKit.Search;
using Microsoft.EntityFrameworkCore.Storage;

public interface IBook
{
    Task<int> SaveAsync();
    Task<Book?> GetJustBookAsync(int bookId, string userId);
    void RemoveAsync(Book book);
    // Task<List<Book>> GetBooksAsync();
    // Task<bool> IsAuthorBookAsync(int bookId, string userId);
    // Task<int> DeleteBookAsync(int bookId);
    Task<int> GetCountBooks();
    Task<Book> GetBookIncludeTagAndGenreAsync(int bookId);
    Task<List<PopularBook>> GetPopularBooks();
    Task<List<PopularBook>> GetLastBooks();
    Task<List<BookDTO>> GetCertainBooksPage(int tag, int genre, int page, string searchText);
    Task<int> GetBooksCountByTagsAndGenres(int tagId, int genreId, string searchText);
    Task<List<MainPopularBook>> GetMainPopularBook();
    Task<DetailBookDTO?> GetBookById(int bookId);
    Task<ShoptBookDTO?> GetShortBookById(int bookId);
    Task<bool> ExistBookAsync(int bookId);
    Task<GenreTagDTO?> GetBookGenresTagsAsync(int bookId);
    Task AddBook(Book book);
    ValueTask<Genre?> GetGenreById(int id);
    ValueTask<Tag?> GetTagById(int id);
    Task<List<MinimalBook>> GetAuthorBooksAsync(string userId);
    Task<List<MinimalBook>> GetSimilarBooksAsync(int bookId, GenreTagDTO book);
}

